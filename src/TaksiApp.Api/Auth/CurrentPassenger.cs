using System.Security.Claims;
using TaksiApp.Domain.Exceptions;

namespace TaksiApp.Api.Auth;

public class CurrentPassenger : ICurrentPassenger
{
    public Guid PassengerId { get; }

    public CurrentPassenger(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var passengerId))
        {
            throw new DomainException("INVALID_ACCESS_TOKEN", "Access token geçersiz veya eksik.", 401);
        }

        PassengerId = passengerId;
    }
}