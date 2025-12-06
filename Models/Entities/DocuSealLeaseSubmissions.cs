using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentEZApi.Models.Entities;

public class DocuSealSubmissions : IIdentifiable, ITimestampedEntity
{
    [Key]
    public Guid Id { get; set; }

    public string? Name { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? ExternalId { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;
    public string? Role { get; set; } = string.Empty;
    public string? OpenedAt { get; set; } = string.Empty;
    public string? CompletedAt { get; set; } = string.Empty;
    public string? DeclinedAt { get; set; } = string.Empty;

    // Foreign Key
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
