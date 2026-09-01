using TaksiApp.Api.Common;

namespace TaksiApp.Api.Extensions;

public static class EndpointExtensions
{
    public static WebApplication UseApiEndpoints(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}