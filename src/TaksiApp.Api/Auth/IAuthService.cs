namespace TaksiApp.Api.Auth;

public interface IAuthService
{
    Task<SendOtpResult> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken);

    Task<VerifyOtpResult> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken);

    Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
}