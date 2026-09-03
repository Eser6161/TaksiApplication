namespace TaksiApp.Api.Features.Auth.Services;

public interface IAuthService
{
    Task SendOtpAsync(string phoneNumber);
    Task<AuthResult> VerifyOtpAsync(string phoneNumber, string otpCode);
    Task CompleteProfileAsync(Guid userId, string fullName, string email);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
}

