using System.Globalization;

namespace RentEZApi.Models.DTOs.Auth;

public class LoginResponseDto
{
    public required string Email { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
}