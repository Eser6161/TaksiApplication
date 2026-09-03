using TaksiApp.Api.Common.Middleware;

namespace TaksiApp.Api.Common.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        // Custom exception handling middleware (development)
        if (app.Environment.IsDevelopment())
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
        else
        {
            // Production exception handler
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }
        
        // CORS
        app.UseCors("AllowAll");
        
        return app;
    }
}