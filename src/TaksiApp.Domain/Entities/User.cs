namespace TaksiApp.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.Passenger;

    // Navigation properties
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}