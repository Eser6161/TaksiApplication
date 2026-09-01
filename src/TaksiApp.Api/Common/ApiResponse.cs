namespace TaksiApp.Api.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Result { get; set; }
    public string? Message { get; set; }
    public ApiError? Error { get; set; }

    public static ApiResponse<T> SuccessResponse(T result, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Result = result,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse(string code, string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = new ApiError(code, message)
        };
    }
}

// Data dönmeyen durumlar için (örn. sadece "silindi" demek istediğinde)
public class ApiResponse : ApiResponse<object?>
{
    public static ApiResponse SuccessResponse(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public static new ApiResponse ErrorResponse(string code, string message)
    {
        return new ApiResponse
        {
            Success = false,
            Error = new ApiError(code, message)
        };
    }
}