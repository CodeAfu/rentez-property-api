namespace RentEZApi.Models.DTOs.Jwt;

public class JwtToken
{
    public required string Name { get; set; }
    public required string Sub { get; set; }
    public required string Role { get; set; }
    public required int Nbf { get; set; }
    public required int Exp { get; set; }
    public required int Iat { get; set; }
    public required string Iss { get; set; }
    public required string Aud { get; set; }
}