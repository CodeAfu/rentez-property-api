using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Template;

public class Submitter
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }
}
