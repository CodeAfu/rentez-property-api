
namespace RentEZApi.Models.DTOs.Property;

public class PropertyFilterRequest
{
    public int PageNum { get; set; } = 1;
    public int Lim { get; set; } = 6;
    public string? Search { get; set; }
    public string? OwnerName { get; set; }
    public string[]? RoomTypes { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public decimal? MinRent { get; set; }
    public decimal? MaxRent { get; set; }
}
