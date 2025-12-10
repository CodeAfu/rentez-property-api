using System.Text.Json.Serialization;
using RentEZApi.Models.DTOs.DocuSeal.Submitter;

namespace RentEZApi.Models.DTOs.DocuSeal.Submission;

public class DocuSealSubmissionResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; } // The submission-level slug

    [JsonPropertyName("submitters")]
    public List<SubmitterResponseDto> Submitters { get; set; } = new();
}
