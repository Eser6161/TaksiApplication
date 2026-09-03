namespace TaksiApp.Api.Features.Auth.DTOs;

public record SendOtpRequest(string PhoneNumber);
public record VerifyOtpRequest(string PhoneNumber, string OtpCode);
public record CompleteProfileRequest(string FullName, string Email);
public record RefreshTokenRequest(string RefreshToken);
