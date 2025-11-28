using RentEZApi.Models.DTOs.Property;

public class PropertyDetails
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public decimal Rent { get; set; }
    public string[] Images { get; set; } = Array.Empty<string>();
    public bool? DepositRequired { get; set; }
    public BillsIncluded BillsIncluded { get; set; } = new();

    public string[] RoomType { get; set; } = Array.Empty<string>();
    public string[] PreferredRaces { get; set; } = Array.Empty<string>();
    public string[] PreferredOccupation { get; set; } = Array.Empty<string>();
    public string[] LeaseTermCategory { get; set; } = Array.Empty<string>();

    public Guid OwnerId { get; set; }
    public Guid? AgreementId { get; set; }
    public DateTime CreatedAt { get; set; }
}
