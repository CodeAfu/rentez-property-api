using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentEZApi.Models.Entities;

[Table("PropertyApplications")]
public class PropertyApplication : IIdentifiable, ITimestampedEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(ApplicantProfile))]
    public Guid ApplicantProfileId { get; set; }
    public ApplicantProfile ApplicantProfile { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Property))]
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    // Timestamp
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
}
