using Microsoft.EntityFrameworkCore;
using TaksiApp.Domain.Entities;
using TaksiApp.Domain.Exceptions;
using TaksiApp.Infrastructure.Persistence;

namespace TaksiApp.Api.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IOtpService _otpService;
    private readonly IJwtTokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        AppDbContext db,
        IOtpService otpService,
        IJwtTokenService tokenService,
        Microsoft.Extensions.Options.IOptions<JwtSettings> jwtOptions)
    {
        _db = db;
        _otpService = otpService;
        _tokenService = tokenService;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<SendOtpResult> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken)
    {
        var expiresIn = await _otpService.SendOtpAsync(
            request.CountryCode, request.PhoneNumber, request.OtpType, request.Role, cancellationToken);

        return new SendOtpResult { ExpiresIn = expiresIn };
    }

    public async Task<VerifyOtpResult> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken)
    {
        await _otpService.VerifyOtpAsync(
            request.CountryCode, request.PhoneNumber, request.OtpType, request.Role, request.Code, cancellationToken);

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
        {
            throw new DomainException("INVALID_ROLE", "Geçersiz rol.", 422);
        }

        var fullPhoneNumber = request.CountryCode.TrimStart('+') + request.PhoneNumber;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == fullPhoneNumber && u.Role == role, cancellationToken);

        if (user is null)
        {
            user = new User
            {
                PhoneNumber = fullPhoneNumber,
                Role = role,
                FullName = string.Empty,
                Email = string.Empty,
                PasswordHash = string.Empty
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var (accessToken, refreshToken) = await IssueTokensAsync(user, cancellationToken);

        return new VerifyOtpResult
        {
            User = new UserDto
            {
                Id = user.Id.ToString(),
                Role = role.ToString().ToLowerInvariant(),
                CountryCode = request.CountryCode,
                PhoneNumber = request.PhoneNumber
            },
            Tokens = new TokenPairDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = _jwtSettings.AccessTokenMinutes * 60
            }
        };
    }

    public async Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashToken(request.RefreshToken);

        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.IsRevoked, cancellationToken);

        if (stored is null || stored.ExpiresAtUtc < DateTime.UtcNow)
        {
            throw new DomainException("INVALID_REFRESH_TOKEN", "Refresh token geçersiz veya süresi dolmuş.", 401);
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == stored.UserId, cancellationToken);

        if (user is null)
        {
            throw new DomainException("INVALID_REFRESH_TOKEN", "Refresh token geçersiz.", 401);
        }

        stored.IsRevoked = true;

        var (accessToken, refreshToken) = await IssueTokensAsync(user, cancellationToken);

        return new RefreshTokenResult
        {
            Tokens = new TokenPairDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = _jwtSettings.AccessTokenMinutes * 60
            }
        };
    }

    private async Task<(string accessToken, string refreshToken)> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(refreshToken),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            IsRevoked = false
        });

        await _db.SaveChangesAsync(cancellationToken);

        return (accessToken, refreshToken);
    }
}