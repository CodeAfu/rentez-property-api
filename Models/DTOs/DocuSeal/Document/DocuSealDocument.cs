using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Document;

public class DocuSealDocument
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
