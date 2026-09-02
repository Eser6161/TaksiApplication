using System.Net;
using System.Text.Json;
using TaksiApp.Domain.Exceptions;

namespace TaksiApp.Api.Common;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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
            _logger.LogError(ex, "Unhandled exception");

            var (statusCode, code, message) = ex switch
            {
                DomainException domainEx => ((HttpStatusCode)domainEx.StatusCode, domainEx.Code, domainEx.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND", ex.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "UNAUTHORIZED", ex.Message),
                ArgumentException => (HttpStatusCode.BadRequest, "BAD_REQUEST", ex.Message),
                _ => (HttpStatusCode.InternalServerError, "INTERNAL_SERVER_ERROR", "İşlem sırasında beklenmeyen bir sunucu hatası oluştu.")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(
                ApiResponse.ErrorResponse(code, message),
                JsonOptions);

            await context.Response.WriteAsync(json);
        }
    }
}
