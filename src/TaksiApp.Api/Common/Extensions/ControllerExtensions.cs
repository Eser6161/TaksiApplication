using System.Text.Json.Serialization;

namespace TaksiApp.Api.Common.Extensions;

public static class ControllerExtensions
{
    public static IServiceCollection AddControllerServices(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            // Global controller options can be configured here
            // For example: model binding, validation, etc.
        })
        .AddJsonOptions(options =>
        {
            // JSON serialization options
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}