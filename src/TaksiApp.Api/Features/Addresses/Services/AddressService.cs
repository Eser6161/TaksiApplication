using TaksiApp.Api.Features.Addresses.DTOs;
using TaksiApp.Api.Features.Auth.Infrastructure;
using TaksiApp.Domain.Entities;
using TaksiApp.Domain.Exceptions;
using TaksiApp.Domain.Interfaces;

namespace TaksiApp.Api.Features.Addresses.Services;

public class AddressService : IAddressService
{
    private readonly IRepository<Address> _addressRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public AddressService(
        IRepository<Address> addressRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AddressDto>> GetUserAddressesAsync()
    {
        var userId = _currentUser.UserId;
        var addresses = await _addressRepository.FindAsync(a => a.UserId == userId);

        return addresses.Select(a => new AddressDto(
            a.Id,
            a.Title,
            a.FullAddress,
            a.Latitude,
            a.Longitude)).ToList();
    }

    public async Task<AddressDto> CreateAddressAsync(CreateAddressRequest request)
    {
        var userId = _currentUser.UserId;

        var address = new Address
        {
            UserId = userId,
            Title = request.Title,
            FullAddress = request.FullAddress,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        await _addressRepository.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();

        return new AddressDto(
            address.Id,
            address.Title,
            address.FullAddress,
            address.Latitude,
            address.Longitude);
    }

    public async Task<AddressDto> UpdateAddressAsync(Guid id, UpdateAddressRequest request)
    {
        var userId = _currentUser.UserId;
        var address = await _addressRepository.GetByIdAsync(id);

        if (address == null || address.UserId != userId)
            throw new DomainException("Address not found or access denied");

        address.Title = request.Title;
        address.FullAddress = request.FullAddress;
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;

        _addressRepository.Update(address);
        await _unitOfWork.SaveChangesAsync();

        return new AddressDto(
            address.Id,
            address.Title,
            address.FullAddress,
            address.Latitude,
            address.Longitude);
    }

    public async Task DeleteAddressAsync(Guid id)
    {
        var userId = _currentUser.UserId;
        var address = await _addressRepository.GetByIdAsync(id);

        if (address == null || address.UserId != userId)
            throw new DomainException("Address not found or access denied");

        _addressRepository.Remove(address);
        await _unitOfWork.SaveChangesAsync();
    }
}