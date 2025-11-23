namespace RentEZApi.Models.DTOs.Result;

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public Pagination Pagination { get; set; } = new();
}
