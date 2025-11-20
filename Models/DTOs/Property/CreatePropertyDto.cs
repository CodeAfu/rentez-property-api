
using RentEZApi.Models.DTOs.Property;

public class CreatePropertyDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required decimal Rent { get; set; }
    public required string[] Images { get; set; } = Array.Empty<string>();
    public bool? DepositRequired { get; set; }
    public BillsIncluded? BillsIncluded { get; set; } = new();
    public Guid? LeaseAgreementId { get; set; }

    // Preferences
    public string[] RoomType { get; set; } = Array.Empty<string>();
    public string[] PreferredRaces { get; set; } = Array.Empty<string>();
    public string[] PreferredOccupation { get; set; } = Array.Empty<string>();
    public string[] LeaseTermCategory { get; set; } = Array.Empty<string>();
}
