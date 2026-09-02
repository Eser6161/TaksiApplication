using Microsoft.EntityFrameworkCore;
using TaksiApp.Domain.Entities;
using TaksiApp.Domain.Exceptions;
using TaksiApp.Infrastructure.Persistence;

namespace TaksiApp.Api.Auth;

public class OtpService : IOtpService
{
    private const int CodeExpiryMinutes = 5;
    private const int MaxAttempts = 5;

    private readonly AppDbContext _db;
    private readonly ILogger<OtpService> _logger;

    public OtpService(AppDbContext db, ILogger<OtpService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<int> SendOtpAsync(string countryCode, string phoneNumber, string otpType, string role, CancellationToken cancellationToken)
    {
        var code = Random.Shared.Next(0, 1_000_000).ToString("D6");

        var otpRequest = new OtpRequest
        {
            CountryCode = countryCode,
            PhoneNumber = phoneNumber,
            OtpType = otpType,
            Role = role,
            Code = code,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(CodeExpiryMinutes),
            IsUsed = false,
            AttemptCount = 0
        };

        _db.OtpRequests.Add(otpRequest);
        await _db.SaveChangesAsync(cancellationToken);

        // MOCK SMS: Gerçek bir SMS sağlayıcısı (Twilio, Netgsm vb.) bağlanana kadar
        // kodu burada logluyoruz. Geliştirme ortamında bu satırdan kodu okuyup test edebilirsin.
        _logger.LogInformation("[MOCK SMS] {CountryCode}{PhoneNumber} numarasına OTP kodu: {Code}", countryCode, phoneNumber, code);

        return CodeExpiryMinutes * 60;
    }

    public async Task VerifyOtpAsync(string countryCode, string phoneNumber, string otpType, string role, string code, CancellationToken cancellationToken)
    {
        var otpRequest = await _db.OtpRequests
            .Where(o => o.CountryCode == countryCode
                        && o.PhoneNumber == phoneNumber
                        && o.OtpType == otpType
                        && o.Role == role
                        && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (otpRequest is null)
        {
            throw new DomainException("OTP_NOT_FOUND", "Geçerli bir OTP isteği bulunamadı, önce kod isteyin.", 422);
        }

        if (otpRequest.ExpiresAtUtc < DateTime.UtcNow)
        {
            throw new DomainException("OTP_EXPIRED", "OTP kodunun süresi dolmuş, yeni kod isteyin.", 422);
        }

        if (otpRequest.AttemptCount >= MaxAttempts)
        {
            throw new DomainException("OTP_MAX_ATTEMPTS_EXCEEDED", "Çok fazla yanlış deneme yapıldı, yeni kod isteyin.", 429);
        }

        if (otpRequest.Code != code)
        {
            otpRequest.AttemptCount++;
            await _db.SaveChangesAsync(cancellationToken);
            throw new DomainException("INVALID_OTP_CODE", "Girilen kod yanlış.", 422);
        }

        otpRequest.IsUsed = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}