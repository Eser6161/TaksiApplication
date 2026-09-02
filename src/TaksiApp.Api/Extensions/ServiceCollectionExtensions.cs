using TaksiApp.Api.Adress;
using TaksiApp.Api.Auth;

namespace TaksiApp.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddHttpContextAccessor();

        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentPassenger, CurrentPassenger>();

        return services;
    }
}