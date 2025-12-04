
namespace RentEZApi.Exceptions;

public class ProfileNotFoundException : Exception
{
    public ProfileNotFoundException()
        : base("Object not found") { }

    public ProfileNotFoundException(Guid id)
        : base($"Object with id '{id}' does not exist") { }

    public ProfileNotFoundException(string message)
        : base(message) { }
}
