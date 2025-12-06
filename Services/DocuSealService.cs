using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;
using RentEZApi.Models.DTOs.DocuSeal;
using RentEZApi.Models.DTOs.DocuSeal.Template;
using RentEZApi.Models.Entities;
using RestSharp;

namespace RentEZApi.Services;

public class DocuSealService
{
    private readonly PropertyDbContext _dbContext;
    private readonly ConfigService _config;
    private readonly ILogger<DocuSealService> _logger;

    public DocuSealService(PropertyDbContext dbContext, ConfigService config, ILogger<DocuSealService> logger)
    {
        _dbContext = dbContext;
        _config = config;
        _logger = logger;
    }

    public async Task<string> GetBuilderToken(string userEmail, string externalId, string propertyId, string? templateId = null)
    {
        _logger.LogInformation($"External ID: {externalId}");
        _logger.LogInformation($"Property ID: {propertyId}");
        _logger.LogInformation($"Template ID: {templateId}");
        var apiKey = _config.GetDocuSealAuthToken()!;
        var secret = Encoding.UTF8.GetBytes(apiKey);
        var payload = new Dictionary<string, object>
        {
            { "exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() }
        };

        if (string.IsNullOrWhiteSpace(userEmail))
        {
            throw new Exception("No email provided");
        }

        if (string.IsNullOrWhiteSpace(externalId))
        {
            throw new Exception("No external ID provided");
        }

        if (!string.IsNullOrEmpty(templateId))
        {
            var dbTemplateId = await _dbContext.Property
                .Where(p => p.Id == Guid.Parse(propertyId))
                .Select(p => p.Agreement != null ? p.Agreement.TemplateId : null)
                .FirstOrDefaultAsync();

            _logger.LogInformation("Fetched Template ID: {Property}", dbTemplateId);

            if (dbTemplateId == null)
            {
                throw new Exception($"Property {propertyId} does not have a valid Agreement/Template.");
            }

            _logger.LogInformation("Set template_id: {TemplateId}", dbTemplateId);
            payload["template_id"] = dbTemplateId;
        }

        _logger.LogInformation("Set user_email: {Email}", userEmail);
        payload["user_email"] = userEmail;

        _logger.LogInformation("Set external_id: {ExternalId}", externalId);
        payload["external_id"] = $"{externalId}:{propertyId}";

        var header = Base64UrlEncode(Encoding.UTF8.GetBytes("{\"alg\":\"HS256\",\"typ\":\"JWT\"}"));
        var payloadJson = Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
        var headerPayload = $"{header}.{payloadJson}";

        using (var hmac = new HMACSHA256(secret))
        {
            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(headerPayload));
            var signature = Base64UrlEncode(signatureBytes);
            return $"{headerPayload}.{signature}";
        }
    }

    public async Task TemplateWebhook(DocuSealWebhookPayload payload)
    {
        _logger.LogInformation("Payload: {JsonPayload}", JsonSerializer.Serialize(payload));
        if (payload.EventType == "template.created" || payload.EventType == "template.updated")
        {
            if (string.IsNullOrEmpty(payload.Data.ExternalId))
                throw new Exception("ExternalId required");

            var parts = payload.Data.ExternalId.Split(':');

            if (parts.Length != 2)
                throw new Exception($"Invalid ExternalId format: {payload.Data.ExternalId}");

            var ownerId = Guid.Parse(parts[0]);
            var propertyId = Guid.Parse(parts[1]);

            var template = await _dbContext.DocuSealPDFTemplates
                .FirstOrDefaultAsync(t => t.TemplateId == payload.Data.Id.ToString());

            if (template != null)
            {
                template.Name = payload.Data.Name;
                template.DocumentJson = JsonSerializer.Serialize(payload.Data);
                template.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                template = new DocuSealPDFTemplate
                {
                    TemplateId = payload.Data.Id.ToString(),
                    Name = payload.Data.Name,
                    DocumentJson = JsonSerializer.Serialize(payload.Data),
                    SubmittersJson = JsonSerializer.Serialize(payload.Data.Submitters),
                    OwnerId = ownerId,
                    CreatedAt = payload.Data.CreatedAt,
                    UpdatedAt = payload.Data.UpdatedAt
                };

                _dbContext.DocuSealPDFTemplates.Add(template);
            }
            var property = await _dbContext.Property
                .FirstOrDefaultAsync(p => p.Id == propertyId);

            if (property != null)
            {
                // Security check: Ensure the property actually belongs to the user mentioned in the token
                if (property.OwnerId != ownerId)
                {
                    _logger.LogWarning($"Security Warning: User {ownerId} tried to attach template to Property {propertyId} owned by {property.OwnerId}");
                    throw new UnauthorizedAccessException("Property owner mismatch");
                }

                // EF Core will automatically update AgreementId when we set this navigation property
                property.Agreement = template;
            }
            else
            {
                _logger.LogWarning($"Property {propertyId} not found for template linking.");
            }
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task SubmissionWebhook(DocuSealWebhookPayload payload)
    {
        _logger.LogInformation("Payload: {JsonPayload}", JsonSerializer.Serialize(payload));
        if (
            payload.EventType != "submission.created" ||
            payload.EventType != "submission.completed" ||
            payload.EventType != "submission.expired" ||
            payload.EventType != "submission.archived"
        ) throw new InvalidOperationException("Invalid webhook event from DocuSeal API");

        if (payload.EventType == "submission.created")
        {

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
