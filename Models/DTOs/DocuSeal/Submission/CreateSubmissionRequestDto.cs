namespace RentEZApi.Models.DTOs.DocuSeal.Submission;

public class CreateSubmissionRequestDto
{
    public Guid PropertyId { get; set; }
    public string TenantEmail { get; set; } = string.Empty;
    public string? Role { get; set; }
    public bool SendEmail { get; set; } = false;
    public bool? SendSMS { get; set; } = false;
    public string? Order { get; set; } = "random";
}

