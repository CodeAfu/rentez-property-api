using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Template;

public class TWHSchemaField
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("submitter")]
    public string? Submitter { get; set; }
    [JsonPropertyName("required")]
    public bool Required { get; set; }
}
