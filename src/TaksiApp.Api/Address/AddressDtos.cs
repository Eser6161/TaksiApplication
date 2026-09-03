namespace TaksiApp.Api.Address;

public record CreateAddressRequest(string Title, string FullAddress, double Latitude, double Longitude);
public record UpdateAddressRequest(string Title, string FullAddress, double Latitude, double Longitude);
