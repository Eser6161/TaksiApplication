using TaksiApp.Api.Features.Addresses.DTOs;

namespace TaksiApp.Api.Features.Addresses.Services;

public interface IAddressService
{
    Task<List<AddressDto>> GetUserAddressesAsync();
    Task<AddressDto> CreateAddressAsync(CreateAddressRequest request);
    Task<AddressDto> UpdateAddressAsync(Guid id, UpdateAddressRequest request);
    Task DeleteAddressAsync(Guid id);
}