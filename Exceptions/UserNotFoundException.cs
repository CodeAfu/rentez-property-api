namespace RentEZApi.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException()
        : base("User not found") { }

    public UserNotFoundException(Guid id)
        : base($"User with id '{id}' does not exist") { }

    public UserNotFoundException(string message)
        : base(message) { }
}
