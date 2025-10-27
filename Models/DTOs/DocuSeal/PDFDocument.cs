namespace RentEZApi.Models.DTOs.DocuSeal;

public class PDFDocument
{
    public required string Name { get; set; }
    public required string File { get; set; } // Base64
    public required List<InputField?> Inputs { get; set; }

}