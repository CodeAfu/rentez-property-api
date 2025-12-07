
using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Document;

public class DocuSealDocumentMetadata
{
    [JsonPropertyName("identified")]
    public bool? Identified { get; set; }

    [JsonPropertyName("analyzed")]
    public bool? Analyzed { get; set; }

    [JsonPropertyName("sha256")]
    public string? Sha256 { get; set; }

    [JsonPropertyName("pdf")]
    public PdfInfo? Pdf { get; set; }


    public class PdfInfo
    {
        [JsonPropertyName("number_of_pages")]
        public int? NumberOfPages { get; set; }
    }
}

