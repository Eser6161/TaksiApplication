using Microsoft.Extensions.DependencyInjection;
using TaksiApp.Application.Address;
using TaksiApp.Application.Auth;

namespace TaksiApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IAddressService, AddressService>();

        return services;
    }
}
