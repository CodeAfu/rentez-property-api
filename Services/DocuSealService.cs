using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
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
    
        var payload = new Dictionary<string, object>
        {
            { "exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() }
        };
        
        if (!string.IsNullOrEmpty(userEmail))
        {
            payload["user_email"] = userEmail;
        }
    
        var secret = Encoding.UTF8.GetBytes(apiKey);
        var securityKey = new SymmetricSecurityKey(secret);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    
        var token = new JwtSecurityToken(
            claims: payload.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? "")),
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );
    
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RestResponse> GetAllTemplates(CancellationToken ct = default)
    {
        var client = new RestClient("https://api.docuseal.com/templates");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("X-Auth-Token", _config.GetDocuSealAuthToken()!);
        var response = await client.ExecuteAsync(request, ct);
        return response;
    }

    public async Task<RestResponse> GetTemplate(string templateId, CancellationToken ct = default)
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
}

