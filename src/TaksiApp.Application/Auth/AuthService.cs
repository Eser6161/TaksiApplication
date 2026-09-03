using TaksiApp.Domain.Entities;
using TaksiApp.Domain.Interfaces;

namespace TaksiApp.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<OtpRequest> _otpRepo;
    private readonly IRepository<RefreshToken> _refreshTokenRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IOtpService _otpService;

    public AuthService(
        IRepository<User> userRepo,
        IRepository<OtpRequest> otpRepo,
        IRepository<RefreshToken> refreshTokenRepo,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IOtpService otpService)
    {
        _userRepo = userRepo;
        _otpRepo = otpRepo;
        _refreshTokenRepo = refreshTokenRepo;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _otpService = otpService;
    }

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
            user = new User
            {
                PhoneNumber = phoneNumber,
                FullName = string.Empty,
                Email = string.Empty,
                PasswordHash = string.Empty
            };
            await _userRepo.AddAsync(user);
        }

        await _unitOfWork.SaveChangesAsync();

        var accessToken = _jwtTokenService.GenerateAccessToken(user!);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user!.Id,
            TokenHash = refreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
        };

        await _refreshTokenRepo.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResult(accessToken, refreshToken, isNewUser);
    }

    public async Task CompleteProfileAsync(Guid userId, string fullName, string email)
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        user.FullName = fullName;
        user.Email = email;

        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        var tokens = await _refreshTokenRepo.FindAsync(r =>
            r.TokenHash == refreshToken &&
            !r.IsRevoked &&
            r.ExpiresAtUtc > DateTime.UtcNow);

        var token = tokens.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş refresh token.");

        token.IsRevoked = true;
        _refreshTokenRepo.Update(token);

        var user = await _userRepo.GetByIdAsync(token.UserId)
            ?? throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
        };

        await _refreshTokenRepo.AddAsync(newRefreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResult(newAccessToken, newRefreshToken, false);
    }
}
