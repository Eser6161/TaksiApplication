using TaksiApp.Domain.Entities;

namespace TaksiApp.Api.Features.Auth.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}

