namespace TaksiApp.Domain.Entities;

public class Address : BaseEntity
{
    public Guid PassengerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AddressText { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Description { get; set; }

    public void Update(string? title, string? addressText, double? latitude, double? longitude, string? description)
    {
        if (title is not null) Title = title;
        if (addressText is not null) AddressText = addressText;
        if (latitude.HasValue) Latitude = latitude.Value;
        if (longitude.HasValue) Longitude = longitude.Value;
        if (description is not null) Description = description;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}