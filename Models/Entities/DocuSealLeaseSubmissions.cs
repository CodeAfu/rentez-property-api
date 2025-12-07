using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentEZApi.Models.Entities;

[Table("DocuSealLeaseSubmissions")]
public class DocuSealLeaseSubmission : IIdentifiable, ITimestampedEntity
{
    [Key]
    public Guid Id { get; set; }
    public long SubmissionId { get; set; }

    public string? Name { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? ExternalId { get; set; } = string.Empty;
    public string? Status { get; set; } = "pending";
    public string? FolderName { get; set; }
    public string? Role { get; set; } = string.Empty;

    // TODO: Store in S3
    // Document (download via webhook)
    public byte[]? DocumentData { get; set; }
    public string? DocumentFileName { get; set; }

    // Foreign Key
    [Required]
    [ForeignKey(nameof(Property))]
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    // Timestamp
    public DateTime? OpenedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeclinedAt { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
}
