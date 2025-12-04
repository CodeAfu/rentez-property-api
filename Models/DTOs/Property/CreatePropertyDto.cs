
using RentEZApi.Models.DTOs.Property;

public class CreatePropertyDto
{
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public decimal Rent { get; set; }
    public string[] Images { get; set; } = Array.Empty<string>();
    public bool? DepositRequired { get; set; }
    public BillsIncluded? BillsIncluded { get; set; } = new();
    public Guid? LeaseAgreementId { get; set; }

    // Preferences
    public string[] RoomType { get; set; } = Array.Empty<string>();
    public string[] PreferredRaces { get; set; } = Array.Empty<string>();
    public string[] PreferredOccupation { get; set; } = Array.Empty<string>();
    public string[] LeaseTermCategory { get; set; } = Array.Empty<string>();
}
