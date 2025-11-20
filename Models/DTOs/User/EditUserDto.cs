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
    public string? Ethnicity { get; set; }
}
