namespace TaksiApp.Domain.Entities;

public class OtpRequest : BaseEntity
{
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string OtpType { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsUsed { get; set; }
    public int AttemptCount { get; set; }
}