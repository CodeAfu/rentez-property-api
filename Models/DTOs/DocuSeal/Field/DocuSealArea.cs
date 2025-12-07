using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Field;

public class DocuSealArea
{
    [JsonPropertyName("x")]
    public decimal X { get; set; }

    [JsonPropertyName("y")]
    public decimal Y { get; set; }

    [JsonPropertyName("w")]
    public decimal W { get; set; }

    [JsonPropertyName("h")]
    public decimal H { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("attachment_uuid")]
    public string? AttachmentUuid { get; set; }
}
