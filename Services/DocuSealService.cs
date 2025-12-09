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
    private readonly ConfigService _config;
    private readonly ILogger<DocuSealService> _logger;
    private readonly HttpClient _httpClient;

    public DocuSealService(PropertyDbContext dbContext, ConfigService config, ILogger<DocuSealService> logger, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
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

    public async Task<string> GetSignerToken(string slug, Guid? propertyId)
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

    // public async Task SaveDocumentSubmission(Guid userId)
    // {
    // }

    // // Template Webhook
    // public async Task TemplateWebhook(DocuSealWebhookPayload payload)
    // {
    //     _logger.LogInformation("Payload: {JsonPayload}", JsonSerializer.Serialize(payload));
    //     if (payload.EventType == "template.created" || payload.EventType == "template.updated")
    //     {
    //         if (string.IsNullOrEmpty(payload.Data.ExternalId))
    //             throw new Exception("ExternalId required");
    //
    //         var parts = payload.Data.ExternalId.Split(':');
    //
    //         if (parts.Length != 2)
    //             throw new Exception($"Invalid ExternalId format: {payload.Data.ExternalId}");
    //
    //         var ownerId = Guid.Parse(parts[0]);
    //         var propertyId = Guid.Parse(parts[1]);
    //
    //         var template = await _dbContext.DocuSealPDFTemplates
    //             .FirstOrDefaultAsync(t => t.TemplateId == payload.Data.Id.ToString());
    //
    //         if (template != null)
    //         {
    //             template.Name = payload.Data.Name;
    //             template.DocumentJson = JsonSerializer.Serialize(payload.Data);
    //             template.UpdatedAt = DateTime.UtcNow;
    //         }
    //         else
    //         {
    //             template = new DocuSealPDFTemplate
    //             {
    //                 TemplateId = payload.Data.Id.ToString(),
    //                 Name = payload.Data.Name,
    //                 DocumentJson = JsonSerializer.Serialize(payload.Data),
    //                 SubmittersJson = JsonSerializer.Serialize(payload.Data.Submitters),
    //                 OwnerId = ownerId,
    //                 CreatedAt = payload.Data.CreatedAt,
    //                 UpdatedAt = payload.Data.UpdatedAt
    //             };
    //
    //             _dbContext.DocuSealPDFTemplates.Add(template);
    //         }
    //         var property = await _dbContext.PropertyListings
    //             .FirstOrDefaultAsync(p => p.Id == propertyId);
    //
    //         if (property != null)
    //         {
    //             // Security check: Ensure the property actually belongs to the user mentioned in the token
    //             if (property.OwnerId != ownerId)
    //             {
    //                 _logger.LogWarning($"Security Warning: User {ownerId} tried to attach template to Property {propertyId} owned by {property.OwnerId}");
    //                 throw new UnauthorizedAccessException("Property owner mismatch");
    //             }
    //
    //             // EF Core will automatically update AgreementId when we set this navigation property
    //             property.Agreement = template;
    //         }
    //         else
    //         {
    //             _logger.LogWarning($"Property {propertyId} not found for template linking.");
    //         }
    //         await _dbContext.SaveChangesAsync();
    //     }
    // }
    //
    // // Submission Webhooks
    // public async Task HandleSubmissionCreated(WebhookData data)
    // {
    //     _logger.LogInformation("Processing submission.created for ID: {Id}", data.Id);
    //
    //     // Get first submitter info (tenant/signer)
    //     var submitter = data.Submitters?.FirstOrDefault();
    //
    //     var propertyId = data.ExternalId?.Split(":")[1];
    //
    //     if (string.IsNullOrEmpty(propertyId))
    //     {
    //         throw new InvalidDataException($"No property ID found: {data.ExternalId}");
    //     }
    //
    //     var submission = new DocuSealLeaseSubmission
    //     {
    //         Id = Guid.NewGuid(),
    //         SubmissionId = data.Id,
    //         Name = submitter?.Name ?? data.Name,
    //         Email = submitter?.Email,
    //         ExternalId = data.ExternalId,
    //         FolderName = data.FolderName,
    //         Status = submitter?.Status ?? "pending",
    //         Role = submitter?.Role,
    //         PropertyId = Guid.Parse(propertyId), // Assuming ExternalId is PropertyId
    //         OpenedAt = ParseDateTime(submitter?.OpenedAt),
    //         CreatedAt = data.CreatedAt,
    //         UpdatedAt = data.UpdatedAt
    //     };
    //
    //     _dbContext.DocuSealLeaseSubmissions.Add(submission);
    //     await _dbContext.SaveChangesAsync();
    //
    //     _logger.LogInformation("Submission created for: {Email}", submission.Email);
    // }
    //
    // public async Task HandleSubmissionCompleted(WebhookData data)
    // {
    //     _logger.LogInformation("Processing submission.completed for ID: {Id}", data.Id);
    //
    //     var submission = await _dbContext.DocuSealLeaseSubmissions
    //         .FirstOrDefaultAsync(s => s.SubmissionId == data.Id);
    //
    //     if (submission == null)
    //     {
    //         throw new InvalidOperationException($"Submission {data.Id} not found");
    //     }
    //
    //     var submitter = data.Submitters?.FirstOrDefault();
    //
    //     submission.Status = "completed";
    //     submission.CompletedAt = ParseDateTime(submitter?.CompletedAt) ?? data.UpdatedAt;
    //     submission.UpdatedAt = data.UpdatedAt;
    //
    //     if (data.Documents != null && data.Documents.Any())
    //     {
    //         var doc = data.Documents.First(); // Assuming one document per submission
    //         if (!string.IsNullOrEmpty(doc.Url))
    //         {
    //             try
    //             {
    //                 var bytes = await _httpClient.GetByteArrayAsync(doc.Url);
    //                 var fileName = $"{submission.Id}_{doc.Name ?? "document"}.pdf";
    //                 var filePath = Path.Combine("storage", "agreements", fileName);
    //
    //                 Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
    //                 await File.WriteAllBytesAsync(filePath, bytes);
    //
    //                 submission.DocumentData = bytes;
    //                 submission.DocumentFileName = doc.Name;
    //                 _logger.LogInformation("Downloaded document: {FileName}", fileName);
    //             }
    //             catch (Exception ex)
    //             {
    //                 _logger.LogError(ex, "Failed to download document: {Url}", doc.Url);
    //                 throw;
    //             }
    //         }
    //     }
    //
    //     await _dbContext.SaveChangesAsync();
    //
    //     _logger.LogInformation("Submission {Id} completed by {Email}", data.Id, submission.Email);
    // }
    //
    // public async Task HandleSubmissionExpired(WebhookData data)
    // {
    //     _logger.LogInformation("Processing submission.expired for ID: {Id}", data.Id);
    //
    //     var submission = await _dbContext.DocuSealLeaseSubmissions
    //         .FirstOrDefaultAsync(s => s.SubmissionId == data.Id);
    //
    //     if (submission == null)
    //     {
    //         throw new InvalidOperationException($"Submission {data.Id} not found");
    //     }
    //
    //     submission.Status = "expired";
    //     submission.UpdatedAt = data.UpdatedAt;
    //
    //     await _dbContext.SaveChangesAsync();
    //
    //     _logger.LogInformation("Submission {Id} expired for {Email}", data.Id, submission.Email);
    // }
    //
    // public async Task HandleSubmissionArchived(WebhookData data)
    // {
    //     _logger.LogInformation("Processing submission.archived for ID: {Id}", data.Id);
    //
    //     var submission = await _dbContext.DocuSealLeaseSubmissions
    //         .FirstOrDefaultAsync(s => s.SubmissionId == data.Id);
    //
    //     if (submission == null)
    //     {
    //         throw new InvalidOperationException($"Submission {data.Id} not found");
    //     }
    //
    //     submission.Status = "archived";
    //     submission.UpdatedAt = data.UpdatedAt;
    //
    //     await _dbContext.SaveChangesAsync();
    //
    //     _logger.LogInformation("Submission {Id} archived", data.Id);
    // }
    //
    // Regular APIs

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

    // public async Task<(bool IsSuccessful, int? TemplateId, HttpStatusCode StatusCode, string? ErrorMessage)> CreateTemplate(PDFDocument document)
    // {
    //     var client = new RestClient("https://api.docuseal.com/templates/pdf");
    //     var request = new RestRequest("", Method.Post);
    //     request.AddHeader("X-Auth-Token", _config.GetDocuSealAuthToken()!);
    //     request.AddHeader("content-type", "application/json");
    //     var payload = new
    //     {
    //         name = document.Name,
    //         documents = new[]
    //         {
    //             new
    //             {
    //                 name = document.Name,
    //                 file = document.File,
    //                 fields = document.Inputs.Select(input => new
    //                 {
    //                     name = input?.Name,
    //                     areas = input?.Areas?.Select(area => new
    //                     {
    //                         x = area.X,
    //                         y = area.Y,
    //                         w = area.W,
    //                         h = area.H,
    //                         page = area.Page
    //                     }).ToArray()
    //                 }).ToArray()
    //             }
    //         }
    //     };
    //     request.AddJsonBody(payload);
    //     var response = await client.ExecuteAsync(request);
    //     if (!response.IsSuccessful)
    //     {
    //         return (false, null, response.StatusCode, response.ErrorMessage);
    //     }
    //     var result = JsonSerializer.Deserialize<DocuSealTemplateResponse>(response.Content!);
    //     return (true, result?.Id, response.StatusCode, null);
    // }

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
