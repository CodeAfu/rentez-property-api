using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;
using RentEZApi.Models.DTOs.DocuSeal.Submission;
using RentEZApi.Models.DTOs.DocuSeal.Submitter;
using RentEZApi.Models.Entities;
using RestSharp;

namespace RentEZApi.Services;

public class DocuSealSubmissionsService
{
    private readonly PropertyDbContext _dbContext;
    private readonly ConfigService _config;
    private readonly EmailService _emailService;
    private readonly ILogger<DocuSealSubmissionsService> _logger;
    private readonly HttpClient _httpClient;

    public DocuSealSubmissionsService(
            PropertyDbContext dbContext,
            ConfigService config,
            EmailService emailService,
            ILogger<DocuSealSubmissionsService> logger,
            IHttpClientFactory httpClientFactory
    )
    {
        _dbContext = dbContext;
        _config = config;
        _emailService = emailService;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<DocuSealSubmitterResponseDto> CreateSubmission(CreateSubmissionRequestDto dto, CancellationToken ct = default)
    {
        // Check if the user already has a submission for this property
        var tenantUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == dto.TenantEmail.ToUpper(), ct);

        if (tenantUser == null)
            throw new InvalidOperationException($"User not found for email: {dto.TenantEmail}");

        // Check if the property has an agreement
        var property = await _dbContext.PropertyListings
                .Include(p => p.Agreement)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == dto.PropertyId, ct);

        if (property?.Agreement == null)
            throw new InvalidOperationException($"Could not fetch TemplateId value for property {dto.PropertyId}. Agreement is possibly not assigned");

        // Check if the user has already sent an email
        var application = await _dbContext.PropertyApplications
                .FirstOrDefaultAsync(pa => pa.PropertyId == dto.PropertyId && pa.UserId == tenantUser.Id, ct);

        if (application == null)
            throw new InvalidOperationException($"Could not find application for {dto.TenantEmail} and {dto.PropertyId}");

        // Create a new submission with docuseal api
        var client = new RestClient("https://api.docuseal.com");
        var request = new RestRequest("submissions", Method.Post);
        request.AddHeader("X-Auth-Token", _config.GetDocuSealAuthToken());
        request.AddHeader("content-type", "application/json");
        var body = new
        {
            template_id = property.Agreement.APITemplateId,
            send_email = false,
            send_sms = false,
            order = dto.Order,
            submitters = new[]
            {
                new
                {
                    role = dto.Role ?? "Tenant",
                    email = dto.TenantEmail
                }
            }
        };
        request.AddJsonBody(body);

        var response = await client.ExecuteAsync(request, ct);
        if (!response.IsSuccessful)
            throw new HttpRequestException(
                    $"DocuSeal API Error ({response.StatusCode}): {response.Content}",
                    null,
                    response.StatusCode
                );

        if (response.Content == null)
            throw new HttpRequestException("No content returned from DocuSeal API");

        _logger.LogInformation("DocuSeal Rest API Response: {Content}", response.Content);

        var submitters = JsonSerializer.Deserialize<List<DocuSealSubmitterResponseDto>>(response.Content);
        if (submitters == null)
            throw new HttpRequestException("No submitters returned from DocuSeal API");

        var firstSubmitter = submitters[0];

        // Create a new submission
        var newSubmission = new DocuSealSubmission
        {
            PropertyId = dto.PropertyId,
            SignerId = tenantUser.Id,
            PropertyApplicationId = application.Id,
            APISubmissionId = firstSubmitter.Id,
            Email = firstSubmitter.Email,
            Role = firstSubmitter.Role,
            SignerSlug = firstSubmitter.Slug, // CRITICAL: Save this for the embedded view
            Status = firstSubmitter.Status,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };

        _dbContext.DocuSealSubmissions.Add(newSubmission);

        if (application != null)
        {
            application.HasSentEmail = true;
        }

        await _dbContext.SaveChangesAsync(ct);

        // Send an email to the tenant
        try
        {
            string frontendBaseUrl = _config.GetWebURL();
            string signingLink = $"{frontendBaseUrl}/lease-submission/{firstSubmitter.Slug}?propertyId={dto.PropertyId}";

            string sender = (property.Owner.FirstName + " " + property.Owner.LastName).Trim();
            string propertyName = property.Title;

            string emailSubject = "RentEZ Property: Lease Agreement Signing Invitation";
            string emailBody = $@"
                <p>Hi there,</p>
                <p>You have been invited to sign a lease document for property <strong>{propertyName}</strong> on RentEZ.</p>
                <p>
                    <a href=""{signingLink}"" style=""padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;"">
                        Review and Sign
                    </a>
                </p>
                <p>Or click here: <a href=""{signingLink}"">{signingLink}</a></p>
                <p>Please contact us by replying to this email if you have any questions.</p>
                <p>Thanks,<br/>{(string.IsNullOrEmpty(sender) ? "RentEZ" : sender)}</p>
            ";

            await _emailService.SendEmail(dto.TenantEmail, emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Submission created but failed to send invitation email to {Email}", dto.TenantEmail);
        }

        return firstSubmitter;
    }

    public async Task<List<GetDocuSealSubmissionDto>> GetSubmissionsByPropertyId(string propertyId, CancellationToken ct = default)
    {
        return await _dbContext.DocuSealSubmissions
                .AsNoTracking()
                .Where(s => s.PropertyId == Guid.Parse(propertyId))
                .Select(s => new GetDocuSealSubmissionDto
                {
                    Id = s.Id,
                    APISubmissionId = s.APISubmissionId,
                    Email = s.Email,
                    Status = s.Status,
                    Role = s.Role,
                    SignerSlug = s.SignerSlug,
                    PropertyId = s.PropertyId,
                    SignerId = s.SignerId,
                    OpenedAt = s.OpenedAt,
                    CompletedAt = s.CompletedAt,
                    DeclinedAt = s.DeclinedAt,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync(ct);
    }

    public async Task<GetDocuSealSubmissionDto?> GetSubmissionByEmailPropId(string submitterEmail, Guid propertyId, CancellationToken ct = default)
    {
        return await _dbContext.DocuSealSubmissions
                .AsNoTracking()
                .Where(s => s.Email == submitterEmail && s.PropertyId == propertyId)
                .Select(s => new GetDocuSealSubmissionDto
                {
                    Id = s.Id,
                    APISubmissionId = s.APISubmissionId,
                    Email = s.Email,
                    Status = s.Status,
                    Role = s.Role,
                    SignerSlug = s.SignerSlug,
                    SubmissionUrl = s.SubmissionUrl,
                    PropertyId = s.PropertyId,
                    SignerId = s.SignerId,
                    OpenedAt = s.OpenedAt,
                    CompletedAt = s.CompletedAt,
                    DeclinedAt = s.DeclinedAt,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .FirstOrDefaultAsync();
    }


    public async Task SignLease(string signerEmail, Guid propertyId, DocuSealLeaseSubmissionRequestDto payload)
    {
        // Check if the submission exists
        var submission = await _dbContext.DocuSealSubmissions
                .FirstOrDefaultAsync(s => s.Email == signerEmail && s.PropertyId == propertyId);

        if (submission == null)
            throw new InvalidOperationException($"Could not find submission for {signerEmail} and {propertyId}");

        // Check if the signer exists
        var signer = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == signerEmail);

        if (signer == null)
            throw new InvalidOperationException($"Could not find signer for {signerEmail}");

        // Get the property application
        var application = await _dbContext.PropertyApplications
                .FirstOrDefaultAsync(pa => pa.UserId == signer.Id && pa.PropertyId == propertyId);

        if (application == null)
            throw new InvalidOperationException($"Could not find application for {signerEmail} and {propertyId}");

        submission.Status = payload.Status;
        submission.OpenedAt = payload.OpenedAt;
        submission.CompletedAt = payload.CompletedAt;
        submission.SubmissionUrl = payload.SubmissionUrl;

        application.HasSignedLease = true;
        application.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<DocuSealSubmission?> GetSubmissionByPropertyIdAndEmail(string propertyId, string? signerEmail)
    {
        return await _dbContext.DocuSealSubmissions
                .FirstOrDefaultAsync(s => s.Email == signerEmail && s.PropertyId == Guid.Parse(propertyId));
    }
}
