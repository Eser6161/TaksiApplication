using System.Net;
using System.Text.Json;
using TaksiApp.Domain.Exceptions;

namespace TaksiApp.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");
            await WriteErrorAsync(context, exception);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            DomainException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "Beklenmeyen bir hata oluştu.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            message
        });

        await context.Response.WriteAsync(payload);
    }
}
