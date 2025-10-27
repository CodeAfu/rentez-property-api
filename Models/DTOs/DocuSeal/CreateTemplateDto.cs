namespace RentEZApi.Models.DTOs.DocuSeal;

public class CreateTemplateDto
{
    public required string Name { get; set; }
    public required List<PDFDocument> Documents { get; set; }
}

