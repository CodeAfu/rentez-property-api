using System.ComponentModel.DataAnnotations;

namespace RentEZApi.Models.DTOs.User;

public class RegisterUserDto
{
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    [MinLength(8)]
    public required string Password { get; set; }
}
