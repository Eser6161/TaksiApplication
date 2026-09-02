using TaksiApp.Domain.Entities;

namespace TaksiApp.Api.Auth;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    string HashToken(string token);
}