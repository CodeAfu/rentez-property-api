namespace RentEZApi.Models.DTOs.DocuSeal.Submission;

public class GetDocuSealSubmissionDto
{
    public Guid Id { get; set; }
    public long APISubmissionId { get; set; }

    public string? Email { get; set; }
    public string? Status { get; set; }
    public string? Role { get; set; }
    public string? SignerSlug { get; set; }
    public string? SubmissionUrl { get; set; }

    public Guid PropertyId { get; set; }
    public Guid SignerId { get; set; }

    public DateTime? OpenedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeclinedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
