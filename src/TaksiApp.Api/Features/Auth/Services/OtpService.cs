using System.Security.Cryptography;

namespace TaksiApp.Api.Features.Auth.Services;

public class OtpService : IOtpService
{
    public string GenerateOtp()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }
}

