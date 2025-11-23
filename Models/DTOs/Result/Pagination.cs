namespace RentEZApi.Models.DTOs.Result;

public class Pagination
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasNextPage => PageNum * PageSize < TotalCount;
    public bool HasPreviousPage => PageNum > 1;
}
