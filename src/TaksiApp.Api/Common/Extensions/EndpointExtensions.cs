namespace TaksiApp.Api.Common.Extensions;

public static class EndpointExtensions
{
    public static WebApplication UseApiEndpoints(this WebApplication app)
    {
        // Development tools
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Custom middleware
        app.UseApiMiddleware();

        // Controller mapping
        app.MapControllers();

        return app;
    }
}