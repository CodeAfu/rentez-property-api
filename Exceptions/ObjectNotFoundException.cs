namespace RentEZApi.Exceptions;

public class ObjectNotFoundException : Exception
{
    public ObjectNotFoundException()
        : base("Object not found") { }
    public ObjectNotFoundException(Guid id)
        : base($"Object with id '{id}' does not exist") { }

    public ObjectNotFoundException(string message)
        : base(message) { }
}
