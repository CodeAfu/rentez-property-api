using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Template;

public class TWHTemplateData
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("external_id")]
    public string? ExternalId { get; set; }
    [JsonPropertyName("folder_name")]
    public string? FolderName { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("schema")]
    public List<TWHSchemaField> Schema { get; set; } = new List<TWHSchemaField>();
    [JsonPropertyName("submitters")]
    public List<TWHSubmitter> Submitters { get; set; } = new List<TWHSubmitter>();
    [JsonPropertyName("documents")]
    public List<TWHDocument> Documents { get; set; } = new List<TWHDocument>();
}
