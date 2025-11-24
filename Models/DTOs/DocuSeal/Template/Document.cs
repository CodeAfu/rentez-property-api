
using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Template;

public class Document
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
