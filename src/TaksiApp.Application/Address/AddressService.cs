using TaksiApp.Domain.Interfaces;
using AddressEntity = TaksiApp.Domain.Entities.Address;

namespace TaksiApp.Application.Address;

public class AddressService : IAddressService
{
    private readonly IRepository<AddressEntity> _addressRepo;
    private readonly IUnitOfWork _unitOfWork;

    public AddressService(IRepository<AddressEntity> addressRepo, IUnitOfWork unitOfWork)
    {
        _addressRepo = addressRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AddressResult>> GetMyAddressesAsync(Guid userId)
    {
        var addresses = await _addressRepo.FindAsync(a => a.UserId == userId);
        return addresses.Select(ToResult).ToList();
    }

    public async Task<AddressResult> CreateAsync(Guid userId, string title, string fullAddress, double latitude, double longitude)
    {
        var address = new AddressEntity
        {
            UserId = userId,
            Title = title,
            FullAddress = fullAddress,
            Latitude = latitude,
            Longitude = longitude
        };

        await _addressRepo.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();
        return ToResult(address);
    }

    public async Task<AddressResult> UpdateAsync(Guid userId, Guid addressId, string title, string fullAddress, double latitude, double longitude)
    {
        var address = await _addressRepo.GetByIdAsync(addressId)
            ?? throw new KeyNotFoundException("Adres bulunamadı.");

        if (address.UserId != userId)
            throw new UnauthorizedAccessException("Bu adres size ait değil.");

        address.Title = title;
        address.FullAddress = fullAddress;
        address.Latitude = latitude;
        address.Longitude = longitude;

        _addressRepo.Update(address);
        await _unitOfWork.SaveChangesAsync();
        return ToResult(address);
    }

    public async Task DeleteAsync(Guid userId, Guid addressId)
    {
        var address = await _addressRepo.GetByIdAsync(addressId)
            ?? throw new KeyNotFoundException("Adres bulunamadı.");

        if (address.UserId != userId)
            throw new UnauthorizedAccessException("Bu adres size ait değil.");

        _addressRepo.Remove(address);
        await _unitOfWork.SaveChangesAsync();
    }

    private static AddressResult ToResult(AddressEntity a)
        => new(a.Id, a.Title, a.FullAddress, a.Latitude, a.Longitude, a.CreatedAtUtc);
}
