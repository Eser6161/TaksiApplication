using TaksiApp.Infrastructure;

namespace TaksiApp.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Controllers and Swagger
        services.AddControllerServices();
        
        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
        
        // Authorization
        services.AddAuthorization();
        
        // Core services
        services.AddHttpContextAccessor();
        
        // Feature-specific services
        services.AddFeatures(configuration);
        
        return services;
    }
}
