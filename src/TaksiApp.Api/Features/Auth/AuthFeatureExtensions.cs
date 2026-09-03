using TaksiApp.Api.Features.Auth.Infrastructure;
using TaksiApp.Api.Features.Auth.Services;

namespace TaksiApp.Api.Features.Auth;

public static class AuthFeatureExtensions
{
    public static IServiceCollection AddAuthFeature(this IServiceCollection services, IConfiguration configuration)
    {
        // Authentication services
        services.AddAuthenticationServices(configuration);
        
        // Feature-specific services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        
        return services;
    }
}