namespace TaksiApp.Api.Features.Addresses.DTOs;

public record AddressDto(
    Guid Id,
    string Title,
    string FullAddress,
    double Latitude,
    double Longitude);

public record CreateAddressRequest(
    string Title,
    string FullAddress,
    double Latitude,
    double Longitude);

public record UpdateAddressRequest(
    string Title,
    string FullAddress,
    double Latitude,
    double Longitude);