using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Submitter;

public class DocuSealSubmitter
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }
}
