using TaksiApp.Domain.Entities;
using TaksiApp.Domain.Exceptions;
using TaksiApp.Domain.Interfaces;

namespace TaksiApp.Api.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<OtpRequest> _otpRepo;
    private readonly IRepository<RefreshToken> _refreshTokenRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IOtpService _otpService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IRepository<User> userRepo,
        IRepository<OtpRequest> otpRepo,
        IRepository<RefreshToken> refreshTokenRepo,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IOtpService otpService,
        IPasswordHasher passwordHasher)
    {
        _userRepo = userRepo;
        _otpRepo = otpRepo;
        _refreshTokenRepo = refreshTokenRepo;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _otpService = otpService;
        _passwordHasher = passwordHasher;
    }

    // ── OTP-based Authentication ──

    public async Task SendOtpAsync(string phoneNumber)
    {
        var otpCode = _otpService.GenerateOtp();

        var otp = new OtpRequest
        {
            CountryCode = "+90",
            PhoneNumber = phoneNumber,
            OtpType = "login",
            Role = nameof(UserRole.Passenger),
            Code = otpCode,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5)
        };

        await _otpRepo.AddAsync(otp);
        await _unitOfWork.SaveChangesAsync();

        // TODO: SMS gönderim entegrasyonu
    }

    public async Task<AuthResult> VerifyOtpAsync(string phoneNumber, string otpCode)
    {
        var otps = await _otpRepo.FindAsync(o =>
            o.PhoneNumber == phoneNumber &&
            o.Code == otpCode &&
            !o.IsUsed &&
            o.ExpiresAtUtc > DateTime.UtcNow);

        var otp = otps.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş OTP.");

        otp.IsUsed = true;
        _otpRepo.Update(otp);

        var users = await _userRepo.FindAsync(u => u.PhoneNumber == phoneNumber);
        var user = users.FirstOrDefault();
        var isNewUser = user is null;

        if (isNewUser)
        {
            user = new User { PhoneNumber = phoneNumber };
            await _userRepo.AddAsync(user);
        }

        await _unitOfWork.SaveChangesAsync();

        return await CreateAuthResultAsync(user!, isNewUser);
    }

    // ── Password-based Authentication ──

    public async Task<AuthResult> RegisterWithPasswordAsync(string fullName, string email, string password, string? phoneNumber)
    {
        var existingUsers = await _userRepo.FindAsync(u => u.Email == email);
        if (existingUsers.Any())
            throw new DomainException("EMAIL_ALREADY_EXISTS", "Bu email adresi zaten kayıtlı.");

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            var phoneUsers = await _userRepo.FindAsync(u => u.PhoneNumber == phoneNumber);
            if (phoneUsers.Any())
                throw new DomainException("PHONE_ALREADY_EXISTS", "Bu telefon numarası zaten kayıtlı.");
        }

        var user = new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(password),
            PhoneNumber = phoneNumber
        };

        await _userRepo.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return await CreateAuthResultAsync(user, true);
    }

    public async Task<AuthResult> LoginWithPasswordAsync(string email, string password)
    {
        var users = await _userRepo.FindAsync(u => u.Email == email);
        var user = users.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Email veya şifre hatalı.");

        if (string.IsNullOrEmpty(user.PasswordHash) || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email veya şifre hatalı.");

        return await CreateAuthResultAsync(user, false);
    }

    // ── Password Management ──

    public async Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        if (string.IsNullOrEmpty(user.PasswordHash) || !_passwordHasher.VerifyPassword(oldPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Mevcut şifre hatalı.");

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ForgotPasswordAsync(string phoneNumber)
    {
        var users = await _userRepo.FindAsync(u => u.PhoneNumber == phoneNumber);
        if (!users.Any())
            throw new KeyNotFoundException("Bu telefon numarasına ait kullanıcı bulunamadı.");

        var otpCode = _otpService.GenerateOtp();

        var otp = new OtpRequest
        {
            CountryCode = "+90",
            PhoneNumber = phoneNumber,
            OtpType = "password_reset",
            Role = nameof(UserRole.Passenger),
            Code = otpCode,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5)
        };

        await _otpRepo.AddAsync(otp);
        await _unitOfWork.SaveChangesAsync();

        // TODO: SMS gönderim entegrasyonu
    }

    public async Task ResetPasswordAsync(string phoneNumber, string otpCode, string newPassword)
    {
        var otps = await _otpRepo.FindAsync(o =>
            o.PhoneNumber == phoneNumber &&
            o.Code == otpCode &&
            o.OtpType == "password_reset" &&
            !o.IsUsed &&
            o.ExpiresAtUtc > DateTime.UtcNow);

        var otp = otps.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş OTP.");

        otp.IsUsed = true;
        _otpRepo.Update(otp);

        var users = await _userRepo.FindAsync(u => u.PhoneNumber == phoneNumber);
        var user = users.FirstOrDefault()
            ?? throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    // ── Profile Management ──

    public async Task CompleteProfileAsync(Guid userId, string fullName, string email)
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        user.FullName = fullName;
        user.Email = email;

        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    // ── Token Management ──

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        var tokens = await _refreshTokenRepo.FindAsync(r =>
            r.Token == refreshToken &&
            !r.IsRevoked &&
            r.ExpiresAtUtc > DateTime.UtcNow);

        var token = tokens.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş refresh token.");

        token.IsRevoked = true;
        _refreshTokenRepo.Update(token);

        var user = await _userRepo.GetByIdAsync(token.UserId)
            ?? throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        return await CreateAuthResultAsync(user, false);
    }

    // ── Private Helpers ──

    private async Task<AuthResult> CreateAuthResultAsync(User user, bool isNewUser)
    {
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
        };

        await _refreshTokenRepo.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResult(accessToken, refreshToken, isNewUser);
    }
}
