namespace TaksiApp.Api.Features.Auth.Services;

/// <summary>
/// BCrypt kullanarak şifre hash'leme ve doğrulama
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // BCrypt ile hash oluştur (work factor: 12)
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        // BCrypt ile doğrula
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
