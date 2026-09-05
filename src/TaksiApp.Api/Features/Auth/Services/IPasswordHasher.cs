namespace TaksiApp.Api.Features.Auth.Services;

/// <summary>
/// Şifre hash'leme ve doğrulama servisi
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Şifreyi hash'ler (BCrypt kullanarak)
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Şifreyi hash ile doğrular
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}
