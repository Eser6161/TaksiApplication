namespace TaksiApp.Application.Address;

public interface IAddressService
{
    Task<List<AddressResult>> GetMyAddressesAsync(Guid userId);
    Task<AddressResult> CreateAsync(Guid userId, string title, string fullAddress, double latitude, double longitude);
    Task<AddressResult> UpdateAsync(Guid userId, Guid addressId, string title, string fullAddress, double latitude, double longitude);
    Task DeleteAsync(Guid userId, Guid addressId);
}
