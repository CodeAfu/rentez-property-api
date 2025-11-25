namespace RentEZApi.Models.DTOs.DocuSeal;

public class InputField
{
    public required string Name { get; set; }
    public List<InputArea>? Areas { get; set; }
}
