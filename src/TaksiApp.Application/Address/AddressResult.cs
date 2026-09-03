namespace TaksiApp.Application.Address;

public record AddressResult(Guid Id, string Title, string FullAddress, double Latitude, double Longitude, DateTime CreatedAtUtc);
