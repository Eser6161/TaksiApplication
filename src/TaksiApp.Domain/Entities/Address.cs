namespace TaksiApp.Domain.Entities;

public class Address : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string FullAddress { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
