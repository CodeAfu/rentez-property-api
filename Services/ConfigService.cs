namespace RentEZApi.Services;
public class ConfigService
{
    private readonly IConfiguration _config;

    public ConfigService(IConfiguration config)
    {
        _config = config;
    }

    public string GetEnvironment() => _config["ASPNETCORE_ENVIRONMENT"] ?? "Production";
    public string? GetDocuSealAuthToken() => _config["DocuSealAuthToken"];
}