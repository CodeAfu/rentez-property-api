using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Template;

public class WebhookData
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("external_id")]
    public string? ExternalId { get; set; }
    [JsonPropertyName("folder_name")]
    public string? FolderName { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("schema")]
    public List<SchemaField> Schema { get; set; } = new List<SchemaField>();
    [JsonPropertyName("submitters")]
    public List<Submitter> Submitters { get; set; } = new List<Submitter>();
    [JsonPropertyName("documents")]
    public List<Document> Documents { get; set; } = new List<Document>();
    [JsonPropertyName("deline_reason")]
    public string? DeclineReason { get; set; }
}
