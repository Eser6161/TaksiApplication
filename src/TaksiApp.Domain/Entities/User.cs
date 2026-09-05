namespace TaksiApp.Domain.Entities;

public class User : BaseEntity
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.Passenger;
}