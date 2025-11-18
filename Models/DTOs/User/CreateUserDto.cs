using System.ComponentModel.DataAnnotations;

namespace RentEZApi.Models.DTOs.User;

public class CreateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Range(18, 120)]
    public int Age { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Occupation { get; set; }
    public string? Ethnicity { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    [MinLength(8)]
    public required string Password { get; set; }
}
