namespace RentEZApi.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException()
        : base("Access not authorized") { }

    public UnauthorizedException(string message)
        : base(message) { }
}
