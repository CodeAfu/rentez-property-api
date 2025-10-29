using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RentEZApi.Models.Entities;

[Index(nameof(TemplateId))]
public class DocuSealPDFTemplate : IIdentifiable, ITimestampedEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string TemplateId { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "jsonb")]
    public string DocumentJson { get; set; } = string.Empty;

    [Required]
    [ForeignKey(nameof(Owner))]
    public Guid OwnerId { get; set; }

    public User Owner { get; set; } = null!;

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
}