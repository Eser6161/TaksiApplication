namespace TaksiApp.Application.Auth;

public record AuthResult(string AccessToken, string RefreshToken, bool IsNewUser);
