namespace RentEZApi.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid id)
        : base($"User with id '{id}' does not exist") { }

    public UserNotFoundException(string message)
        : base(message) { }
}
