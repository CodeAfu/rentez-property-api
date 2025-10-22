namespace RentEZApi.Services;
public class ConfigService
{
    private readonly IConfiguration _config;

    public ConfigService(IConfiguration config)
    {
        _config = config;
    }

    public string GetEnvironment()
    {
        return _config["ASPNETCORE_ENVIRONMENT"] ?? "Production";
    }
}