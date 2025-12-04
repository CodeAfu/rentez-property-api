using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Template;

public class DocuSealWebhookPayload
{
    [JsonPropertyName("event_type")]
    public required string EventType { get; set; }
    [JsonPropertyName("timestamp")]
    public required string Timestamp { get; set; }
    [JsonPropertyName("data")]
    public required WebhookData Data { get; set; }
}
