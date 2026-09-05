using TaksiApp.Api.Common.Middleware;

namespace TaksiApp.Api.Common.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        // Custom exception handling — tüm ortamlarda aktif
        // DomainException, UnauthorizedAccessException, KeyNotFoundException
        // hepsini yakalar ve uygun HTTP status code ile döner
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // CORS
        app.UseCors("AllowAll");

        return app;
    }
}