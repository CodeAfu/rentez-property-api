namespace RentEZApi.Exceptions;

public class DbUpdateException : Exception
{
    public DbUpdateException()
        : base() { }

    public DbUpdateException(string message)
        : base(message) { }
}
