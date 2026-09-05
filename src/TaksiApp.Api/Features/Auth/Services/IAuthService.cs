namespace TaksiApp.Api.Features.Auth.Services;

public interface IAuthService
{
    // OTP-based Authentication
    Task SendOtpAsync(string phoneNumber);
    Task<AuthResult> VerifyOtpAsync(string phoneNumber, string otpCode);
    
    // Password-based Authentication
    Task<AuthResult> RegisterWithPasswordAsync(string fullName, string email, string password, string? phoneNumber);
    Task<AuthResult> LoginWithPasswordAsync(string email, string password);
    
    // Password Management
    Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
    Task ForgotPasswordAsync(string phoneNumber);
    Task ResetPasswordAsync(string phoneNumber, string otpCode, string newPassword);
    
    // Profile Management
    Task CompleteProfileAsync(Guid userId, string fullName, string email);
    
    // Token Management
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
}

