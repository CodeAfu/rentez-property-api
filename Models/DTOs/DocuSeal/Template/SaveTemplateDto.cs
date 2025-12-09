using RentEZApi.Models.DTOs.DocuSeal.Document;
using RentEZApi.Models.DTOs.DocuSeal.Field;
using RentEZApi.Models.DTOs.DocuSeal.Submitter;
using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Template;

public class TemplatePayloadDto
{
    [JsonPropertyName("id")]
    public long TemplateId { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("fields")]
    public List<DocuSealField> Fields { get; set; } = new();

    [JsonPropertyName("submitters")]
    public List<DocuSealSubmitter> Submitters { get; set; } = new();

    [JsonPropertyName("documents")]
    public List<DocuSealDocument> Documents { get; set; } = new();

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
