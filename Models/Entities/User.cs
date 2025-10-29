using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RentEZApi.Models.Entities;

[Table("Users")]
public class User : IdentityUser<Guid>, IIdentifiable, ITimestampedEntity
{
    // [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    // [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    // [Required]
    [Range(18, 120)]
    public int Age { get; set; }

    // [Required]
    [MaxLength(100)]
    public string Occupation { get; set; } = string.Empty;

    // [Required]
    [MaxLength(50)]
    public string Ethnicity { get; set; } = string.Empty;

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    public ICollection<DocuSealPDFTemplate> Templates { get; set; } = new List<DocuSealPDFTemplate>();
}