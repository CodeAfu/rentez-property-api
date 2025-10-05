namespace RentEZApi.Models.DTOs;

public class UserDto : IIdentifiable, ITimestampedEntity
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int Age { get; set; }

    public required string Occupation { get; set; }
    public required string Ethnicity { get; set; }
    public required string EmailAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
