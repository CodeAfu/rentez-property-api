namespace RentEZApi.Models.DTOs.PropertyApplication;

public class PropertyApplicationResponse
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public Guid ApplicantProfileId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
