namespace RentEZApi.Models.Response;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }
    public string? Message { get; }
    
    private Result(bool isSuccess, T? data, string? error, string? message)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        Message = message;
    }
    
    public static Result<T> Success(T data, string? message = null)
        => new(true, data, null, message);
    
    public static Result<T> Failure(string error, string? message = null)
        => new(false, default, error, message);
}