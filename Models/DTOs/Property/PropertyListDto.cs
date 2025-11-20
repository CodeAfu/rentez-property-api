
namespace RentEZApi.Models.DTOs.Property;

public class PropertyListDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string OwnerFirstName { get; set; }
    public required string OwnerLastName { get; set; }
    public decimal Rent { get; set; }
    public string[] Images { get; set; } = Array.Empty<string>();
    public string[] RoomType { get; set; } = Array.Empty<string>();
}
