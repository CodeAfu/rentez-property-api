using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Submitter;

public class DocuSealTemplateSubmitter
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }
}
