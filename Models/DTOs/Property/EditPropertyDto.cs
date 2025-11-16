

namespace RentEZApi.Models.DTOs.Property;

public class EditPropertyDto
{
    public string? Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string? City { get; set; } = string.Empty;
    public string[]? Images { get; set; }
    public bool? DepositRequired { get; set; }
    public BillsIncluded? BillsIncluded { get; set; }
    public string[]? RoomType { get; set; } = Array.Empty<string>();
    public string[]? PreferredRaces { get; set; } = Array.Empty<string>();
    public string[]? PreferredOccupation { get; set; } = Array.Empty<string>();
    public string[]? LeaseTermCategory { get; set; } = Array.Empty<string>();
}
