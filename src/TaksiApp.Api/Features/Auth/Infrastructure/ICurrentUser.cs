namespace TaksiApp.Api.Features.Auth.Infrastructure;

public interface ICurrentUser
{
    Guid UserId { get; }
}
