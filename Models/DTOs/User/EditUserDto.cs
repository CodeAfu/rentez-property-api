using System.ComponentModel.DataAnnotations;

namespace RentEZApi.Models.DTOs.User;

public class EditUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Range(18, 120)]
    public int Age { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Occupation { get; set; }
    public string? Ethnicity { get; set; }
}