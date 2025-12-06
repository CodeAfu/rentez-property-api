using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Template;

public class SubmitterDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("external_id")]
    public string? ExternalId { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("opened_at")]
    public string? OpenedAt { get; set; }
    [JsonPropertyName("completed_at")]
    public string? CompletedAt { get; set; }
    [JsonPropertyName("declined_at")]
    public string? DeclinedAt { get; set; }
    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public string? UpdatedAt { get; set; }
}
