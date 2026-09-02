using System.ComponentModel.DataAnnotations;

namespace TaksiApp.Api.Adress;

public class AddressDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AddressText { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class AddressListResult
{
    public List<AddressDto> Addresses { get; set; } = new();
}

public class AddAddressRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string AddressText { get; set; } = string.Empty;

    [Required, Range(-90, 90)]
    public double Latitude { get; set; }

    [Required, Range(-180, 180)]
    public double Longitude { get; set; }
}

public class AddAddressResult
{
    public AddressDto Address { get; set; } = new();
}

public class UpdateAddressRequest
{
    public string? Title { get; set; }
    public string? AddressText { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Description { get; set; }
}

public class DeleteAddressResult
{
    public string DeletedAddressId { get; set; } = string.Empty;
}