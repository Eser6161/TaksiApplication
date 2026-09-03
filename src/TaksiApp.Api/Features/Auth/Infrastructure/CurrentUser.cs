using System.Security.Claims;
using TaksiApp.Domain.Exceptions;

namespace TaksiApp.Api.Features.Auth.Infrastructure;

public class CurrentUser : ICurrentUser
{
    public Guid UserId { get; }

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var passengerId))
        {
            throw new DomainException("INVALID_ACCESS_TOKEN", "Access token geçersiz veya eksik.", 401);
        }

        UserId = passengerId;
    }
}
