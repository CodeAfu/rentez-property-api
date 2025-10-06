using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentEZApi.Models.Entities;

[Table("users")]
public class User : IIdentifiable, ITimestampedEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    [Column("first_name")]
    public required string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("last_name")]
    public required string LastName { get; set; }

    [Required]
    [Range(18, 120)]
    [Column("age")]
    public required int Age { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("occupation")]
    public required string Occupation { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("phone_number")]
    public required string PhoneNumber { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("ethnicity")]
    public required string Ethnicity { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("email_address")]
    public required string EmailAddress { get; set; }

    [Required]
    [MinLength(8)]
    [MaxLength(255)]
    [Column("password_hash")]
    public required string PasswordHash { get; set; }

    [Required]
    [Column("created_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}