using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Submission;

public class DocuSealLeaseSubmissionRequestDto
{
    [JsonPropertyName("submission_url")]
    public string SubmissionUrl { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("opened_at")]
    public DateTime OpenedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime CompletedAt { get; set; }

}

