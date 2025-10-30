namespace RentEZApi.Models.DTOs.Auth;

public class JwtInfo
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Key { get; set; }
    public int TokenValidityMins { get; set; }
    public DateTime TokenExpiryTimestamp { get; set; }
}