using System.Text.Json.Serialization;

namespace TaksiApp.Api.Common.Extensions;

public static class ControllerExtensions
{
    public static IServiceCollection AddControllerServices(this IServiceCollection services)
    {
        services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // JWT Bearer token desteği — Swagger UI'da "Authorize" butonu
                        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.ParameterLocation.Header,
                Description = "JWT token'inizi girin. Örnek: eyJhbGciOiJIUzI1NiIs..."
            });

            options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
            {
                [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer")] = new List<string>()
            });
        });

        return services;
    }
}   