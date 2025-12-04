namespace RentEZApi.Models.DTOs.User;

public class UpdateApplicantProfileRequest
{
    public decimal? MonthlyIncome { get; set; }
    public string? EmployerName { get; set; }
    public string? GovernmentIdType { get; set; }
    public string? GovernmentIdNumber { get; set; }
    public int? NumberOfOccupants { get; set; }
    public bool? HasPets { get; set; }
    public string? PetDetails { get; set; }
}
