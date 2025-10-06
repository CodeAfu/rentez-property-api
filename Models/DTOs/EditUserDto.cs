namespace Models.DTOs;

public class EditUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Occupation { get; set; }
    public string? Ethnicity { get; set; }
}