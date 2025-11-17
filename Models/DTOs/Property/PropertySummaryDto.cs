namespace RentEZApi.Models.DTOs.Property;

public class PropertySummaryDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public decimal Rent { get; set; }
}
