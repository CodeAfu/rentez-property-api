using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public static class Hasher
{
    public static string Hash(object obj)
    {
        if (obj == null) return string.Empty;

        // Serialize object to JSON
        string json = JsonSerializer.Serialize(obj);

        using var sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        byte[] hashBytes = sha256.ComputeHash(bytes);

        // Convert hash to hex string
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}
