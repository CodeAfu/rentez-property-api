using Microsoft.AspNetCore.Identity;

namespace RentEZApi.Services;

public class AuthorizationService
{
    private static readonly PasswordHasher<object> _passwordHasher = new();
    public static string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    public static bool VerifyPassword(string passwordHash, string password)
    {
        return _passwordHasher.VerifyHashedPassword(null!, passwordHash, password)
            == PasswordVerificationResult.Success;
    }
}