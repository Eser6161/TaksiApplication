using TaksiApp.Api.Features.Addresses.Services;

namespace TaksiApp.Api.Features.Addresses;

public static class AddressesFeatureExtensions
{
    public static IServiceCollection AddAddressesFeature(this IServiceCollection services)
    {
        // Feature-specific services
        services.AddScoped<IAddressService, AddressService>();
        
        return services;
    }
}