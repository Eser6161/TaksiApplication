using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaksiApp.Application.Auth;

namespace TaksiApp.Api.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentPassenger _currentPassenger;

    public AuthController(IAuthService authService, ICurrentPassenger currentPassenger)
    {
        _authService = authService;
        _currentPassenger = currentPassenger;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        await _authService.SendOtpAsync(request.PhoneNumber);
        return Ok(new { message = "OTP gönderildi." });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var result = await _authService.VerifyOtpAsync(request.PhoneNumber, request.OtpCode);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("complete-profile")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request)
    {
        await _authService.CompleteProfileAsync(_currentPassenger.UserId, request.FullName, request.Email);
        return Ok(new { message = "Profil tamamlandı." });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(result);
    }
}