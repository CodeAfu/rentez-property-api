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
    public string? GetDocuSealAuthToken() => _config["DocuSealAuthToken"];
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