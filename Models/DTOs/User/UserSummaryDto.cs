namespace RentEZApi.Models.DTOs.User;

public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Ethnicity { get; set; }
    public string? Occupation { get; set; }
    public string? Email { get; set; }
}
