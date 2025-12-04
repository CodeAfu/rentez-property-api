using RentEZApi.Attributes;

namespace RentEZApi.Models.DTOs.User;

public class EditUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [MinimumAge(18)]
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Occupation { get; set; }

    // Application Profile
    public decimal? MonthlyIncome { get; set; }
    public string? EmployerName { get; set; }
    public string? GovernmentIdType { get; set; }
    public string? GovernmentIdNumber { get; set; }
    public int? NumberOfOccupants { get; set; }
    public bool? HasPets { get; set; }
    public string? PetDetails { get; set; }
    public string? Ethnicity { get; set; }
}
