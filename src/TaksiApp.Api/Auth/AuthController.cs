using Microsoft.AspNetCore.Mvc;
using TaksiApp.Api.Common;

namespace TaksiApp.Api.Auth;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.SendOtpAsync(request, cancellationToken);
        return Ok(new ApiResponse<SendOtpResult>
        {
            Success = true,
            Result = result,
            Message = "OTP kodu gönderildi"
        });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.VerifyOtpAsync(request, cancellationToken);
        return Ok(new ApiResponse<VerifyOtpResult>
        {
            Success = true,
            Result = result,
            Message = "Giriş başarılı"
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);
        return Ok(new ApiResponse<RefreshTokenResult>
        {
            Success = true,
            Result = result,
            Message = "Token yenilendi"
        });
    }
}