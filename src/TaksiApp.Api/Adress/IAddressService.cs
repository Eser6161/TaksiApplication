namespace TaksiApp.Api.Adress;

public interface IAddressService
{
    Task<List<AddressDto>> GetAddressesAsync(Guid passengerId, CancellationToken cancellationToken);

    Task<AddressDto> AddAddressAsync(Guid passengerId, AddAddressRequest request, CancellationToken cancellationToken);

    Task<AddressDto> UpdateAddressAsync(Guid passengerId, Guid addressId, UpdateAddressRequest request, CancellationToken cancellationToken);

    Task<string> DeleteAddressAsync(Guid passengerId, Guid addressId, CancellationToken cancellationToken);
}