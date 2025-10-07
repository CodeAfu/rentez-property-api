namespace RentEZApi.Models.DTOs;

public class UserAuthDto
{
    public required string EmailAddress { get; set; }
    public required string Password { get; set; }
}