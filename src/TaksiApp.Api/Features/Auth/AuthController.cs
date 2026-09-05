using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaksiApp.Api.Features.Auth.DTOs;
using TaksiApp.Api.Features.Auth.Infrastructure;
using TaksiApp.Api.Features.Auth.Services;

namespace TaksiApp.Api.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUser _currentUser;

    public AuthController(IAuthService authService, ICurrentUser currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    // ── OTP-based Authentication ──

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

    // ── Password-based Authentication ──

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterWithPasswordRequest request)
    {
        var result = await _authService.RegisterWithPasswordAsync(
            request.FullName, request.Email, request.Password, request.PhoneNumber);
        return StatusCode(201, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginWithPasswordRequest request)
    {
        var result = await _authService.LoginWithPasswordAsync(request.Email, request.Password);
        return Ok(result);
    }

    // ── Password Management ──

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        await _authService.ChangePasswordAsync(_currentUser.UserId, request.OldPassword, request.NewPassword);
        return Ok(new { message = "Şifre başarıyla değiştirildi." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(request.PhoneNumber);
        return Ok(new { message = "Şifre sıfırlama OTP'si gönderildi." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _authService.ResetPasswordAsync(request.PhoneNumber, request.OtpCode, request.NewPassword);
        return Ok(new { message = "Şifre başarıyla sıfırlandı." });
    }

    // ── Profile Management ──

    [Authorize]
    [HttpPost("complete-profile")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request)
    {
        await _authService.CompleteProfileAsync(_currentUser.UserId, request.FullName, request.Email);
        return Ok(new { message = "Profil tamamlandı." });
    }

    // ── Token Management ──

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(result);
    }
}
