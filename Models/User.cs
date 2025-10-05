using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentEZApi.Models;

[Table("users")]
public class User : IIdentifiable, ITimestampedEntity
{
    [Key]
    [MaxLength(36)]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

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
    [MaxLength(255)]
    [Column("email_address")]
    public string EmailAddress { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("created_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}