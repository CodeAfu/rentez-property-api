

namespace RentEZApi.Models.DTOs.DocuSeal.Template;

public class SaveTemplateResponseDto
{
    public long TemplateId { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
