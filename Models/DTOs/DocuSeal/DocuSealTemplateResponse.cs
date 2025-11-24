using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal;

public class DocuSealTemplateResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
