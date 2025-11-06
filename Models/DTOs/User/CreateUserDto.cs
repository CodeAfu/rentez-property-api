using System.ComponentModel.DataAnnotations;

namespace RentEZApi.Models.DTOs.User;

public class CreateUserDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    [Range(18, 120)]
    public required int Age { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Occupation { get; set; }
    public required string Ethnicity { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    [MinLength(8)]
    public required string Password { get; set; }
}
