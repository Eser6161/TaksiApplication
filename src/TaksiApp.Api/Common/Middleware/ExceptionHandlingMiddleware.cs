using System.Net;
using System.Text.Json;
using TaksiApp.Domain.Exceptions;

namespace TaksiApp.Api.Common.Middleware;

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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            DomainException domainEx => new
            {
                statusCode = domainEx.StatusCode,
                error = domainEx.Code,
                message = domainEx.Message
            },
            UnauthorizedAccessException => new
            {
                statusCode = (int)HttpStatusCode.Unauthorized,
                error = "UNAUTHORIZED",
                message = exception.Message
            },
            KeyNotFoundException => new
            {
                statusCode = (int)HttpStatusCode.NotFound,
                error = "NOT_FOUND",
                message = exception.Message
            },
            _ => new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                error = "INTERNAL_SERVER_ERROR",
                message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin."
            }
        };

        context.Response.StatusCode = response.statusCode;

        _logger.LogError(exception, "Hata yakalandı: {Message}", exception.Message);

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
