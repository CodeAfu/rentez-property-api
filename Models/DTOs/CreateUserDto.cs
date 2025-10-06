namespace RentEZApi.Models.DTOs;

public class CreaterUserDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int Age { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Occupation { get; set; }
    public required string Ethnicity { get; set; }
    public required string EmailAddress { get; set; }
    public required string PasswordHash { get; set; }

}
