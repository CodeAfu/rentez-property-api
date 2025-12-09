using RentEZApi.Models.DTOs.Auth;

namespace RentEZApi.Services;

public class ConfigService
{
    private readonly IConfiguration _config;

    public ConfigService(IConfiguration config)
    {
        _config = config;
    }

    public string? Get(string key) => _config[key];

    public string GetEnvironment() => _config["ASPNETCORE_ENVIRONMENT"] ?? "Production";
    public bool IsDevelopment() => _config["ASPNETCORE_ENVIRONMENT"] == "Development";

    public string GetDocuSealAuthToken() =>
        _config["DocuSeal:AuthToken"] ?? throw new InvalidOperationException("DocuSeal API Key is missing");

    public string GetWebhookSecret() =>
        _config["DocuSeal:WebhookSecret"] ?? throw new InvalidOperationException("DocuSeal Webhook Secret is missing");

    public string GetProdEmail() =>
        _config["ProdEmail"] ?? throw new InvalidOperationException("Production Email is missing");

    public string GetWebURL() =>
        _config["WebURL"] ?? throw new InvalidOperationException("Web URL is missing");

    // Optional: Returns string? (nullable)
    public string? GetTestEmail() => _config["TestEmail"];

    public JwtInfo GetJwtInfo()
    {
        var tokenValidityMins = int.Parse(_config["Jwt:TokenValidityMins"]
            ?? throw new InvalidOperationException("JWT TokenValidityMins is not configured"));

        return new JwtInfo
        {
            Issuer = _config["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer is not configured"),
            Audience = _config["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience is not configured"),
            Key = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is not configured"),
            TokenValidityMins = tokenValidityMins,
            TokenExpiryTimestamp = DateTime.UtcNow.AddMinutes(tokenValidityMins)
        };
    }
}
