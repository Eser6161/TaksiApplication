using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaksiApp.Api.Common.Models;
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

    [HttpPost("sendOtp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        await _authService.SendOtpAsync(request.PhoneNumber);
        return Ok(ApiResponse<object?>.SuccessResponse(null, "OTP gönderildi."));
    }

    [HttpPost("verifyOtp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var result = await _authService.VerifyOtpAsync(request.PhoneNumber, request.OtpCode);
        return Ok(ApiResponse<AuthResult>.SuccessResponse(result, result.IsNewUser ? "Yeni kullanıcı oluşturuldu." : "Giriş başarılı."));
    }

    // ── Password-based Authentication ──

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterWithPasswordRequest request)
    {
        var result = await _authService.RegisterWithPasswordAsync(
            request.FullName, request.Email, request.Password, request.PhoneNumber);
        return StatusCode(201, ApiResponse<AuthResult>.SuccessResponse(result, "Kayıt başarılı."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginWithPasswordRequest request)
    {
        var result = await _authService.LoginWithPasswordAsync(request.Email, request.Password);
        return Ok(ApiResponse<AuthResult>.SuccessResponse(result, "Giriş başarılı."));
    }

    // ── Password Management ──

    [Authorize]
    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        await _authService.ChangePasswordAsync(_currentUser.UserId, request.OldPassword, request.NewPassword);
        return Ok(ApiResponse<object?>.SuccessResponse(null, "Şifre başarıyla değiştirildi."));
    }

    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(request.PhoneNumber);
        return Ok(ApiResponse<object?>.SuccessResponse(null, "Şifre sıfırlama OTP'si gönderildi."));
    }

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _authService.ResetPasswordAsync(request.PhoneNumber, request.OtpCode, request.NewPassword);
        return Ok(ApiResponse<object?>.SuccessResponse(null, "Şifre başarıyla sıfırlandı."));
    }

    // ── Profile Management ──

    [Authorize]
    [HttpPost("completeProfile")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request)
    {
        await _authService.CompleteProfileAsync(_currentUser.UserId, request.FullName, request.Email);
        return Ok(ApiResponse<object?>.SuccessResponse(null, "Profil tamamlandı."));
    }

    // ── Token Management ──

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(ApiResponse<AuthResult>.SuccessResponse(result, "Token yenilendi."));
    }
}