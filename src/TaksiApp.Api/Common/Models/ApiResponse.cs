namespace TaksiApp.Api.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Result { get; set; }
    public string? Message { get; set; }

    public static ApiResponse<T> SuccessResponse(T result, string? message = null)
        => new() { Success = true, Result = result, Message = message };
}

public class ApiErrorDetail
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ApiResponse
{
    public bool Success { get; set; }
    public ApiErrorDetail? Error { get; set; }

    public static ApiResponse ErrorResponse(string code, string message)
        => new() { Success = false, Error = new ApiErrorDetail { Code = code, Message = message } };
}