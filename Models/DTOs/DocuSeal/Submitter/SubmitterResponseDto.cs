using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Submitter;

public class SubmitterResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("submission_id")]
    public long SubmissionId { get; set; }

    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; } // The signer-specific slug (Use this for embedding)

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("opened_at")]
    public DateTime? OpenedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }
}
