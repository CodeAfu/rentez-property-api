using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RentEZApi.Data;
using RentEZApi.Models.Entities;

namespace RentEZApi.Services;

public class RefreshTokenService
{
    private readonly PropertyDbContext _dbContext;

    public RefreshTokenService(PropertyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddToken(Guid userId, string tokenHash)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();
    }

    // Generate token to send to client
    public static string GenerateToken()
    {
        byte[] randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }
    
    // Create hash to store in database
    public static string HashToken(string token)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
            byte[] hashBytes = sha256.ComputeHash(tokenBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
    
    // Verify token against stored hash
    public static bool VerifyToken(string providedToken, string storedHash)
    {
        string providedHash = HashToken(providedToken);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(providedHash),
            Encoding.UTF8.GetBytes(storedHash)
        );
    }
}