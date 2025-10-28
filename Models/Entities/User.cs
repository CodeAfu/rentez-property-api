using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RentEZApi.Models.Entities;

[Table("users")]
public class User : IdentityUser<Guid>, IIdentifiable, ITimestampedEntity
{
    [Required]
    [MaxLength(50)]
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Range(18, 120)]
    [Column("age")]
    public int Age { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("occupation")]
    public string Occupation { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("ethnicity")]
    public string Ethnicity { get; set; } = string.Empty;

    [Required]
    [Column("created_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public ICollection<DocuSealPDFTemplate> Templates { get; set; } = new List<DocuSealPDFTemplate>();
}