using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RentEZApi.Exceptions;
using RentEZApi.Models.DTOs.Auth;
using RentEZApi.Models.Entities;

namespace RentEZApi.Services;

public class JwtService
{
    private readonly ConfigService _config;
    private readonly UsersService _usersService;
    private readonly UserManager<User> _userManager;
    public JwtService(UsersService usersService, ConfigService config, UserManager<User> userManager)
    {
        _usersService = usersService;
        _config = config;
        _userManager = userManager;
    }

    public async Task<LoginResponseDto?> Authenticate(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return null;

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
            throw new UserNotAuthorizedException("Invalid credentials");

        _usersService.VerifyPassword(user.PasswordHash, request.Password);

        var roles = await _userManager.GetRolesAsync(user);
    
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, request.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };
        
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));
        
        var jwtInfo = _config.GetJwtInfo();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = jwtInfo.TokenExpiryTimestamp,
            Issuer = jwtInfo.Issuer,
            Audience = jwtInfo.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtInfo.Key)),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        return new LoginResponseDto
        {
            Email = request.Email,
            AccessToken = accessToken,
            ExpiresIn = (int) jwtInfo.TokenExpiryTimestamp.Subtract(DateTime.UtcNow).TotalSeconds
        };
    }
}