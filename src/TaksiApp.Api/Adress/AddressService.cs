using Microsoft.EntityFrameworkCore;
using TaksiApp.Domain.Exceptions;
using TaksiApp.Infrastructure.Persistence;
using DomainAddress = TaksiApp.Domain.Entities.Address;

namespace TaksiApp.Api.Adress;

public class AddressService : IAddressService
{
    private const int MaxAddressesPerPassenger = 10;

    private readonly AppDbContext _db;

    public AddressService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<AddressDto>> GetAddressesAsync(Guid passengerId, CancellationToken cancellationToken)
    {
        return await _db.Addresses
            .Where(a => a.PassengerId == passengerId)
            .Select(a => ToDto(a))
            .ToListAsync(cancellationToken);
    }

    public async Task<AddressDto> AddAddressAsync(Guid passengerId, AddAddressRequest request, CancellationToken cancellationToken)
    {
        var currentCount = await _db.Addresses
            .CountAsync(a => a.PassengerId == passengerId, cancellationToken);

        if (currentCount >= MaxAddressesPerPassenger)
        {
            throw new DomainException("ADDRESS_LIMIT_EXCEEDED", "Maksimum kayıtlı adres sayısına ulaşıldı.", 409);
        }

        var duplicateTitle = await _db.Addresses
            .AnyAsync(a => a.PassengerId == passengerId && a.Title == request.Title, cancellationToken);

        if (duplicateTitle)
        {
            throw new DomainException("DUPLICATE_ADDRESS_TITLE", "Bu başlıkla kayıtlı bir adres zaten var.", 409);
        }

        var address = new DomainAddress
        {
            PassengerId = passengerId,
            Title = request.Title,
            AddressText = request.AddressText,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        _db.Addresses.Add(address);
        await _db.SaveChangesAsync(cancellationToken);

        return ToDto(address);
    }

    public async Task<AddressDto> UpdateAddressAsync(Guid passengerId, Guid addressId, UpdateAddressRequest request, CancellationToken cancellationToken)
    {
        if (request.Title is null && request.AddressText is null && request.Latitude is null
            && request.Longitude is null && request.Description is null)
        {
            throw new DomainException("VALIDATION_ERROR", "Güncellemek için en az bir alan gönderilmeli.", 422);
        }

        var address = await _db.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.PassengerId == passengerId, cancellationToken);

        if (address is null)
        {
            throw new DomainException("ADDRESS_NOT_FOUND", "Adres bulunamadı.", 404);
        }

        if (request.Title is not null && request.Title != address.Title)
        {
            var duplicateTitle = await _db.Addresses
                .AnyAsync(a => a.PassengerId == passengerId && a.Id != addressId && a.Title == request.Title, cancellationToken);

            if (duplicateTitle)
            {
                throw new DomainException("DUPLICATE_ADDRESS_TITLE", "Bu başlıkla kayıtlı bir adres zaten var.", 409);
            }
        }

        address.Update(request.Title, request.AddressText, request.Latitude, request.Longitude, request.Description);

        await _db.SaveChangesAsync(cancellationToken);

        return ToDto(address);
    }

    public async Task<string> DeleteAddressAsync(Guid passengerId, Guid addressId, CancellationToken cancellationToken)
    {
        var address = await _db.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.PassengerId == passengerId, cancellationToken);

        if (address is null)
        {
            throw new DomainException("ADDRESS_NOT_FOUND", "Adres bulunamadı.", 404);
        }

        // TODO: Aktif bir yolculukta kullanılan adres varsa burada ADDRESS_IN_USE kontrolü eklenmeli.
        // Şu an ride/trip entity'si bilinmediği için bu kontrol atlandı.

        _db.Addresses.Remove(address);
        await _db.SaveChangesAsync(cancellationToken);

        return address.Id.ToString();
    }

    private static AddressDto ToDto(DomainAddress address)
    {
        return new AddressDto
        {
            Id = address.Id.ToString(),
            Title = address.Title,
            AddressText = address.AddressText,
            Latitude = address.Latitude,
            Longitude = address.Longitude
        };
    }
}