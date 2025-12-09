namespace RentEZApi.Models.DTOs.DocuSeal.Submission;

public class CreateSubmissionDto
{
    public Guid PropertyId { get; set; }
    public Guid TenantId { get; set; }
    public long TemplateId { get; set; }
    public bool SendEmail { get; set; }
    public bool? SendSMS { get; set; }
    public string? Order { get; set; }
}

