using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentEZApi.Models.Entities;

public class DocuSealTemplate : IIdentifiable, ITimestampedEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public long APITemplateId { get; set; }

    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;


    [Column(TypeName = "jsonb")]
    public string? DocumentsJson { get; set; }

    [Column(TypeName = "jsonb")]
    public string? FieldsJson { get; set; }

    [Column(TypeName = "jsonb")]
    public string? SubmittersJson { get; set; }


    [Required]
    [ForeignKey(nameof(Owner))]
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public Property? Property { get; set; }


    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

}
