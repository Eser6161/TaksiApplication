namespace TaksiApp.Domain.Exceptions;

public class DomainException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public DomainException(string message)
        : this("DOMAIN_ERROR", message, 400)
    {
    }

    public DomainException(string code, string message, int statusCode = 400)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }
}