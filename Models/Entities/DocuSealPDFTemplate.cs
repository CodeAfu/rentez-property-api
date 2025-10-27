using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RentEZApi.Models.Entities;

[Table("docuseal_pdf_templates")]
[Index(nameof(TemplateId))]
public class DocuSealPDFTemplate : IIdentifiable, ITimestampedEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    [Column("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    [Required]
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("document", TypeName = "jsonb")]
    public string DocumentJson { get; set; } = string.Empty;

    [Required]
    [Column("owner_id")]
    [ForeignKey(nameof(Owner))]
    public Guid OwnerId { get; set; }

    public User Owner { get; set; } = null!;

    [Required]
    [Column("created_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}