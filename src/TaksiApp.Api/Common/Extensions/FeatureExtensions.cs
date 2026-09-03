using TaksiApp.Api.Features.Addresses;
using TaksiApp.Api.Features.Auth;

namespace TaksiApp.Api.Common.Extensions;

public static class FeatureExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        // Register all feature-specific services
        services.AddAuthFeature(configuration);
        services.AddAddressesFeature();
        
        // Health feature doesn't need additional services
        
        return services;
    }
}