
using System.Text.Json;

namespace RentEZApi.Utils;

public static class JsonUtils
{
    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, GetJsonSerializerOptions());
    }

    public static string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, GetJsonSerializerOptions());
    }

    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };
    }
}
