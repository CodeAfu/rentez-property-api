using RentEZApi.Models.DTOs.Property;

namespace RentEZApi.Models.DTOs.User;

public class SelectUserDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Ethnicity { get; set; }
    public string? Occupation { get; set; }
    public string? Email { get; set; }
    public UserApplicantProfileSummary? ApplicantProfile { get; set; }
    public required ICollection<PropertySummaryDto> OwnedProperty { get; set; }
    public decimal? MonthlyIncome { get; set; }
    public string? EmployerName { get; set; }
    public string? GovernmentIdType { get; set; }
    public string? GovernmentIdNumber { get; set; }
    public int? NumberOfOccupants { get; set; }
    public bool? HasPets { get; set; }
    public string? PetDetails { get; set; }
}
