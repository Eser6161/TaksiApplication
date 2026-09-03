using TaksiApp.Domain.Entities;

namespace TaksiApp.Application.Auth;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
