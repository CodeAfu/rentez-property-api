using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using RentEZApi.Attributes;

namespace RentEZApi.Models.Entities;

[Table("Users")]
public class User : IdentityUser<Guid>, IIdentifiable, ITimestampedEntity
{
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [MinimumAge(18)]
    public DateTime? DateOfBirth { get; set; }

    // [NotMapped]
    // public int? Age => DateOfBirth.HasValue
    //     ? DateTime.UtcNow.Year - DateOfBirth.Value.Year -
    //         (DateTime.UtcNow.DayOfYear < DateOfBirth.Value.DayOfYear ? 1 : 0)
    //     : null;

    [MaxLength(100)]
    public string Occupation { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Ethnicity { get; set; } = string.Empty;

    // Application Profile
    public decimal? MonthlyIncome { get; set; }
    public string? EmployerName { get; set; } = string.Empty;
    // public string EmployerContact { get; set; } = string.Empty;

    // public string? PreviousAddress { get; set; } = string.Empty;
    // public string? LandlordName { get; set; } = string.Empty;
    // public string? LandlordContact { get; set; } = string.Empty;

    public string? GovernmentIdType { get; set; } = string.Empty;
    public string? GovernmentIdNumber { get; set; } = string.Empty;

    // public bool BackgroundCheckConsent { get; set; }
    // public bool CreditCheckConsent { get; set; }

    public int? NumberOfOccupants { get; set; }
    public bool? HasPets { get; set; }
    public string? PetDetails { get; set; } = string.Empty;

    // Foreign Keys
    public ICollection<Property> OwnedProperty { get; set; } = new List<Property>();

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    public ICollection<DocuSealPDFTemplate> Templates { get; set; } = new List<DocuSealPDFTemplate>();
}
