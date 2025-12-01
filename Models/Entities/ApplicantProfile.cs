using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentEZApi.Models.Entities;

[Table("ApplicantProfiles")]
public class ApplicantProfile : IIdentifiable, ITimestampedEntity
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Financial
    public decimal? MonthlyIncome { get; set; }
    public string? EmployerName { get; set; } = string.Empty;
    // public string EmployerContact { get; set; } = string.Empty;

    // Rental History
    // public string? PreviousAddress { get; set; } = string.Empty;
    // public string? LandlordName { get; set; } = string.Empty;
    // public string? LandlordContact { get; set; } = string.Empty;

    // Identity (encrypt sensitive fields)
    public string? GovernmentIdType { get; set; } = string.Empty;
    public string? GovernmentIdNumber { get; set; } = string.Empty;

    // Consents
    // public bool BackgroundCheckConsent { get; set; }
    // public bool CreditCheckConsent { get; set; }

    // Additional
    public int? NumberOfOccupants { get; set; }
    public bool? HasPets { get; set; }
    public string? PetDetails { get; set; } = string.Empty;

    // Timestamp
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
}
