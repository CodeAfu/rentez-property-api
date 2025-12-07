using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Field;

public class DocuSealField
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("areas")]
    public List<DocuSealArea> Areas { get; set; } = new();
}
