namespace TaksiApp.Api.Common;

public class ApiError{

    public string Message { get; set; } = default!;
    public string Code { get; set; } = default!;
   
    public ApiError(string message, string code){
        Message = message;
        Code = code;
    }

   
}