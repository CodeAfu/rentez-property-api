using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Submission;

public class DocuSealLeaseSubmissionRequestDto
{
    [JsonPropertyName("submission_url")]
    public required string SubmissionUrl { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonPropertyName("opened_at")]
    public DateTime OpenedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime CompletedAt { get; set; }

}

