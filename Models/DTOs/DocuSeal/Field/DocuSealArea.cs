using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Field;

public class DocuSealArea
{
    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("w")]
    public int W { get; set; }

    [JsonPropertyName("h")]
    public int H { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }
}
