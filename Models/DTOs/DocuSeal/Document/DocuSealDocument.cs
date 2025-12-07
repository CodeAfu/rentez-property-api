using System.Text.Json.Serialization;

namespace RentEZApi.Models.DTOs.DocuSeal.Document;

public class DocuSealDocument
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }

    [JsonPropertyName("record_type")]
    public string? RecordType { get; set; }

    [JsonPropertyName("record_id")]
    public long? RecordId { get; set; }

    [JsonPropertyName("blob_id")]
    public long? BlobId { get; set; }

    [JsonPropertyName("metadata")]
    public DocuSealDocumentMetadata? Metadata { get; set; }
}
