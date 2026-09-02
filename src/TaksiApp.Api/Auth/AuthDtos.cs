using System.ComponentModel.DataAnnotations;

namespace TaksiApp.Api.Auth;

public class SendOtpRequest
{
    [Required]
    public string CountryCode { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string OtpType { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}

public class SendOtpResult
{
    public int ExpiresIn { get; set; }
}

public class VerifyOtpRequest
{
    [Required]
    public string CountryCode { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string OtpType { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class TokenPairDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
}

public class VerifyOtpResult
{
    public UserDto User { get; set; } = new();
    public TokenPairDto Tokens { get; set; } = new();
}

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenResult
{
    public TokenPairDto Tokens { get; set; } = new();
}