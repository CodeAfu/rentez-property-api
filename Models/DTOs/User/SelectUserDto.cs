using RentEZApi.Models.DTOs.Property;

namespace RentEZApi.Models.DTOs.User;

public class SelectUserDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Ethnicity { get; set; }
    public required string Email { get; set; }
    public required ICollection<PropertySummaryDto> OwnedProperty { get; set; }
}
