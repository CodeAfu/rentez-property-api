using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentEZApi.Data;
using RentEZApi.Exceptions;
using RentEZApi.Models.DTOs.Auth;
using RentEZApi.Models.Entities;

namespace RentEZApi.Services;

public class JwtService
{
    private readonly ConfigService _config;
    private readonly UsersService _usersService;
    private readonly UserManager<User> _userManager;
    private readonly PropertyDbContext _dbContext;

    public JwtService(
        UsersService usersService,
        ConfigService config,
        UserManager<User> userManager,
        PropertyDbContext dbContext)
    {
        _usersService = usersService;
        _config = config;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<LoginResponseDto> Authenticate(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) 
            throw new UserNotFoundException($"User with email '{request.Email}' not found");
        if (string.IsNullOrWhiteSpace(user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials");

        _usersService.VerifyPassword(user.PasswordHash, request.Password);
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = CreateAccessToken(request.Email, user.Id, roles);
        var refreshToken = await CreateRefreshToken(user.Id);

        return new LoginResponseDto
        {
            Email = request.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = (int) _config.GetJwtInfo().TokenExpiryTimestamp.Subtract(DateTime.UtcNow).TotalSeconds
        };
    }

    private string CreateAccessToken(string requestEmail, Guid userId, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, requestEmail),
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
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

        return accessToken;
    }

    public async Task<string> CreateRefreshToken(Guid userId)
    {
        // Revoke old tokens
        var oldTokens = await _dbContext.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync();

        foreach (var old in oldTokens)
                old.RevokedAt = DateTime.UtcNow;
            
        string token = RefreshTokenService.GenerateToken();
        string hash = RefreshTokenService.HashToken(token);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = hash,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return token;
    }

    public async Task<LoginResponseDto> RefreshAccessToken(string providedRefreshToken)
    {
        var hash = RefreshTokenService.HashToken(providedRefreshToken);
        
        var storedToken = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == hash);
        
        if (storedToken?.IsValid != true)
            throw new UnauthorizedException("Invalid refresh token");
        
        var roles = await _userManager.GetRolesAsync(storedToken.User);
        var accessToken = CreateAccessToken(storedToken.User.Email!, storedToken.UserId, roles);
        
        var newRefreshToken = await CreateRefreshToken(storedToken.UserId);
        
        return new LoginResponseDto
        {
            Email = storedToken.User.Email!,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = (int) _config.GetJwtInfo().TokenExpiryTimestamp.Subtract(DateTime.UtcNow).TotalSeconds
        };
    }
}