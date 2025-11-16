using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using RentEZApi.Models.DTOs.Property;

namespace RentEZApi.Models.Entities;

public class Property : IIdentifiable, ITimestampedEntity, IUnique
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(64)]
    public required string Hash { get; set; }

    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [MaxLength(50)]
    public string State { get; set; } = string.Empty;

    public decimal Rent { get; set; }

    public string[] Images { get; set; } = Array.Empty<string>();

    public bool? DepositRequired { get; set; }

    public BillsIncluded BillsIncluded { get; set; } = new();

    // Preferences
    public string[] RoomType { get; set; } = Array.Empty<string>();
    public string[] PreferredRaces { get; set; } = Array.Empty<string>();
    public string[] PreferredOccupation { get; set; } = Array.Empty<string>();
    public string[] LeaseTermCategory { get; set; } = Array.Empty<string>();

    // Navigation
    [Required]
    [ForeignKey(nameof(Owner))]
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public Guid? AgreementId { get; set; }
    public DocuSealPDFTemplate? Agreement { get; set; }

    // Timestamp
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
}
