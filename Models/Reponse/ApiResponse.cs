namespace RentEZApi.Models.Response;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }

    public static ApiResponse Fail(string error, string? message = null)
        => new() { Success = false, Error = error, Message = message };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> FromData(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };
}