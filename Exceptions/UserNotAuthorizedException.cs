namespace RentEZApi.Exceptions;

public class UserNotAuthorizedException : Exception
{
    public UserNotAuthorizedException()
        : base() { }

    public UserNotAuthorizedException(string message)
        : base(message) { }
}
