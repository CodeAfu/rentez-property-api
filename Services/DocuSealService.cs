using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RentEZApi.Data;
using RentEZApi.Models.DTOs.DocuSeal;
using RestSharp;

namespace RentEZApi.Services;

public class DocuSealService
{
    private readonly PropertyDbContext _dbContext;
    private readonly ConfigService _config;

    public DocuSealService(PropertyDbContext dbContext, ConfigService config)
    {
        _dbContext = dbContext;
        _config = config;
    }

    public string GetBuilderToken(string? userEmail = null)
    {
        var apiKey = _config.GetDocuSealAuthToken()!;
        var secret = Encoding.UTF8.GetBytes(apiKey);

        var payload = new Dictionary<string, object>
        {
            { "exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() }
        };

        if (!string.IsNullOrEmpty(userEmail))
        {
            payload["user_email"] = userEmail;
        }

        var header = Base64UrlEncode(Encoding.UTF8.GetBytes("{\"alg\":\"HS256\",\"typ\":\"JWT\"}"));
        var payloadJson = Base64UrlEncode(Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(payload)));
        var headerPayload = $"{header}.{payloadJson}";

        using (var hmac = new HMACSHA256(secret))
        {
            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(headerPayload));
            var signature = Base64UrlEncode(signatureBytes);
            return $"{headerPayload}.{signature}";
        }
    }

    public async Task<RestResponse> GetAllTemplates(CancellationToken ct = default)
    {
        var client = new RestClient("https://api.docuseal.com/templates");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("X-Auth-Token", _config.GetDocuSealAuthToken()!);
        var response = await client.ExecuteAsync(request, ct);
        return response;
    }

    public async Task<RestResponse> GetTemplateDetails(string templateId, CancellationToken ct = default)
    {
        var client = new RestClient($"https://api.docuseal.com/templates/{templateId}");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("X-Auth-Token", _config.GetDocuSealAuthToken()!);
        var response = await client.ExecuteAsync(request, ct);
        return response;
    }

    public async Task<(bool IsSuccessful, int? TemplateId, HttpStatusCode StatusCode, string? ErrorMessage)> CreateTemplate(PDFDocument document)
    {
        var client = new RestClient("https://api.docuseal.com/templates/pdf");
        var request = new RestRequest("", Method.Post);
        request.AddHeader("X-Auth-Token", _config.GetDocuSealAuthToken()!);
        request.AddHeader("content-type", "application/json");
        var payload = new
        {
            name = document.Name,
            documents = new[]
            {
                new
                {
                    name = document.Name,
                    file = document.File,
                    fields = document.Inputs.Select(input => new
                    {
                        name = input?.Name,
                        areas = input?.Areas?.Select(area => new
                        {
                            x = area.X,
                            y = area.Y,
                            w = area.W,
                            h = area.H,
                            page = area.Page
                        }).ToArray()
                    }).ToArray()
                }
            }
        };
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            return (false, null, response.StatusCode, response.ErrorMessage);
        }
        var result = JsonSerializer.Deserialize<DocuSealTemplateResponse>(response.Content!);
        return (true, result?.Id, response.StatusCode, null);
    }

    private static string Base64UrlEncode(byte[] data)
    {
        var base64 = Convert.ToBase64String(data);
        return base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
