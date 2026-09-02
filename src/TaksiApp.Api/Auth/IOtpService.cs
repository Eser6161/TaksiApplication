namespace TaksiApp.Api.Auth;

public interface IOtpService
{
    Task<int> SendOtpAsync(string countryCode, string phoneNumber, string otpType, string role, CancellationToken cancellationToken);

    Task VerifyOtpAsync(string countryCode, string phoneNumber, string otpType, string role, string code, CancellationToken cancellationToken);
}