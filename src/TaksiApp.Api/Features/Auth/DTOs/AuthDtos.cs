namespace TaksiApp.Api.Features.Auth.DTOs;

// OTP-based Authentication
public record SendOtpRequest(string PhoneNumber);
public record VerifyOtpRequest(string PhoneNumber, string OtpCode);

// Password-based Authentication
public record RegisterWithPasswordRequest(string FullName, string Email, string Password, string? PhoneNumber);
public record LoginWithPasswordRequest(string Email, string Password);
public record ChangePasswordRequest(string OldPassword, string NewPassword);
public record ForgotPasswordRequest(string PhoneNumber);
public record ResetPasswordRequest(string PhoneNumber, string OtpCode, string NewPassword);

// Profile Management
public record CompleteProfileRequest(string FullName, string Email);

// Token Management
public record RefreshTokenRequest(string RefreshToken);


