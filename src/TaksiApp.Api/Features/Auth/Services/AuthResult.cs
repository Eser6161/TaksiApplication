namespace TaksiApp.Api.Features.Auth.Services;

public record AuthResult(string AccessToken, string RefreshToken, bool IsNewUser);

