using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentEZApi.Models.Entities;

[Table("PropertyApplications")]
public class PropertyApplication : IIdentifiable, ITimestampedEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public decimal RentAmount { get; set; } // For calculating owed amounts over the tenancy period
    // [Required]
    // public decimal RentPaid { get; set; }

    public bool HasSignedLease { get; set; } = false;
    public bool HasSentEmail { get; set; } = false;
    public bool IsRenting { get; set; } = false;

    // Foreign Keys
    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Property))]
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public DocuSealSubmission? DocuSealSubmission { get; set; }

    // Timestamp
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
}
