using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;
using RentEZApi.Models.DTOs.DocuSeal.Template;
using RentEZApi.Models.Entities;
using RestSharp;

namespace RentEZApi.Services;

public class DocuSealService
{
    private readonly PropertyDbContext _dbContext;
    private readonly EmailService _emailService;
    private readonly ConfigService _config;
    private readonly ILogger<DocuSealService> _logger;
    private readonly HttpClient _httpClient;

    public DocuSealService(PropertyDbContext dbContext, EmailService emailService, ConfigService config, ILogger<DocuSealService> logger, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _config = config;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<string> GetBuilderToken(string userEmail, string userId, string propertyId, string? templateId = null)
    {
        _logger.LogInformation($"External ID: {userId}");
        _logger.LogInformation($"Property ID: {propertyId}");
        _logger.LogInformation($"Template ID: {templateId}");

        var apiKey = _config.GetDocuSealAuthToken()
            ?? throw new InvalidOperationException("DocuSeal API key not configured");

        var secret = Encoding.UTF8.GetBytes(apiKey);
        var payload = new Dictionary<string, object>
        {
            { "exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() }
        };

        if (string.IsNullOrWhiteSpace(userEmail))
        {
            throw new Exception("No email provided");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new Exception("No external ID provided");
        }

        if (!string.IsNullOrEmpty(templateId))
        {
            var dbTemplateId = await _dbContext.PropertyListings
                .Where(p => p.Id == Guid.Parse(propertyId))
                .Select(p => p.Agreement != null ? p.Agreement.APITemplateId : (long?)null)
                .FirstOrDefaultAsync();

            if (dbTemplateId == null)
            {
                throw new Exception($"Property {propertyId} does not have a valid Agreement/Template.");
            }

            _logger.LogInformation("Set template_id: {TemplateId}", dbTemplateId);
            payload["template_id"] = dbTemplateId.ToString()!;
        }

        _logger.LogInformation("Set user_email: {Email}", userEmail);
        payload["user_email"] = userEmail;

        _logger.LogInformation("Set external_id: {ExternalId}", userId);
        payload["external_id"] = $"{userId}:{propertyId}";

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

    public string GetSignerToken(string slug, Guid? propertyId)
    {
        _logger.LogInformation("Slug: {Slug}", slug);
        _logger.LogInformation("Property ID: {PropertyId}", propertyId);

        if (propertyId == null)
            throw new InvalidOperationException("propertyId is set to null");

        var apiKey = _config.GetDocuSealAuthToken()
            ?? throw new InvalidOperationException("DocuSeal API key not configured");

        var secret = Encoding.UTF8.GetBytes(apiKey);
        var payload = new Dictionary<string, object>
        {
            { "slug", slug },
            { "exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() },
        };

        var header = Base64UrlEncode(Encoding.UTF8.GetBytes("{\"alg\":\"HS256\",\"typ\":\"JWT\"}"));
        var payloadJson = Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
        var headerPayload = $"{header}.{payloadJson}";

        using var hmac = new HMACSHA256(secret);
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(headerPayload));
        var signature = Base64UrlEncode(signatureBytes);

        return $"{headerPayload}.{signature}";
    }

    public async Task<SaveTemplateResponseDto> SaveDocuSealTemplate(Guid propertyId, Guid templateId, Guid userId, TemplatePayloadDto dto)
    {
        var property = await _dbContext.PropertyListings
            .Include(p => p.Agreement)
            .FirstOrDefaultAsync(p => p.Id == propertyId);

        if (property == null)
            throw new InvalidOperationException($"Property {propertyId} not found");

        if (property.OwnerId != userId)
            throw new UnauthorizedAccessException($"User {userId} does not own property {propertyId}");

        if (property.AgreementId.HasValue && property.AgreementId != templateId)
            throw new InvalidOperationException($"Property already linked to different template");

        var agreement = await _dbContext.DocuSealTemplates
            .FirstOrDefaultAsync(t => t.Id == templateId);

        if (agreement == null)
        {
            agreement = new DocuSealTemplate
            {
                APITemplateId = dto.TemplateId,
                Name = dto.Name,
                Slug = dto.Slug,
                DocumentsJson = JsonSerializer.Serialize(dto.Documents),
                SubmittersJson = JsonSerializer.Serialize(dto.Submitters),
                FieldsJson = JsonSerializer.Serialize(dto.Fields),
                OwnerId = userId,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.DocuSealTemplates.Add(agreement);
            property.AgreementId = agreement.Id;
        }
        else
        {
            agreement.APITemplateId = dto.TemplateId;
            agreement.Name = dto.Name;
            agreement.Slug = dto.Slug;
            agreement.DocumentsJson = JsonSerializer.Serialize(dto.Documents);
            agreement.SubmittersJson = JsonSerializer.Serialize(dto.Submitters);
            agreement.FieldsJson = JsonSerializer.Serialize(dto.Fields);
            agreement.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        return new SaveTemplateResponseDto()
        {
            TemplateId = agreement.APITemplateId,
            OwnerId = agreement.OwnerId,
            Name = agreement.Name,
            Slug = agreement.Slug,
        };
    }

    public async Task<CreateTemplateResponseDto> CreateDocuSealTemplate(Guid userId, Guid propertyId, TemplatePayloadDto dto)
    {
        var property = await _dbContext.PropertyListings
            .Include(p => p.Agreement)
            .FirstOrDefaultAsync(p => p.Id == propertyId);

        if (property == null)
            throw new InvalidOperationException($"Property {propertyId} not found");

        if (property.OwnerId != userId)
            throw new UnauthorizedAccessException($"User {userId} does not own property {propertyId}");

        if (property.AgreementId.HasValue)
            return new CreateTemplateResponseDto()
            {
                Created = false,
                TemplateId = property.AgreementId.Value,
            };

        var agreement = new DocuSealTemplate
        {
            APITemplateId = dto.TemplateId,
            Name = dto.Name,
            Slug = dto.Slug,
            DocumentsJson = JsonSerializer.Serialize(dto.Documents),
            SubmittersJson = JsonSerializer.Serialize(dto.Submitters),
            FieldsJson = JsonSerializer.Serialize(dto.Fields),
            OwnerId = userId,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.DocuSealTemplates.Add(agreement);
        property.AgreementId = agreement.Id;

        await _dbContext.SaveChangesAsync();

        return new CreateTemplateResponseDto()
        {
            Created = true,
            TemplateId = agreement.Id,
        };
    }

    // Regular APIs

    public async Task<RestResponse> GetAllTemplates(CancellationToken ct = default)
    {
        var client = new RestClient("https://api.docuseal.com/templates");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("X-Auth-Token", _config.GetDocuSealAuthToken());
        var response = await client.ExecuteAsync(request, ct);
        return response;
    }

    public async Task<RestResponse> GetTemplateDetails(string templateId, CancellationToken ct = default)
    {
        var client = new RestClient($"https://api.docuseal.com/templates/{templateId}");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("X-Auth-Token", _config.GetDocuSealAuthToken());
        var response = await client.ExecuteAsync(request, ct);
        return response;
    }

    private static string Base64UrlEncode(byte[] data)
    {
        var base64 = Convert.ToBase64String(data);
        return base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static DateTime? ParseDateTime(string? dateTimeString)
    {
        if (string.IsNullOrEmpty(dateTimeString))
            return null;

        return DateTime.TryParse(dateTimeString, out var result)
            ? result.ToUniversalTime()
            : null;
    }
}
