using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentEZApi.Models.DTOs.DocuSeal;
using RentEZApi.Models.DTOs.DocuSeal.Template;
using RentEZApi.Services;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocuSealController : ControllerBase
{
    private readonly DocuSealService _docuSealService;
    private readonly ConfigService _config;
    private readonly ILogger<DocuSealController> _logger;

    public DocuSealController(DocuSealService docuSealService, ConfigService config, ILogger<DocuSealController> logger)
    {
        _docuSealService = docuSealService;
        _config = config;
        _logger = logger;
    }

    [HttpPost("builder-token")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public IActionResult GetBuilderToken([FromQuery] string propertyId, [FromQuery] string? templateId)
    {
        // var adminEmail = _config.GetTestEmail();
        var adminEmail = _config.GetProdEmail();
        if (string.IsNullOrEmpty(adminEmail))
        {
            _logger.LogError("Please setup a 'ProdEmail` environment variable with your registered DocuSeal email");
            return Conflict(new
            {
                message = "No email detected"
            });
        }

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized(new
            {
                message = "Please login to use this feature"
            });

        try
        {
            var tokenString = _docuSealService.GetBuilderToken(adminEmail, currentUserId, propertyId, templateId);
            return Ok(new { token = tokenString });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new
            {
                message = "Something went wrong"
            });
        }
    }

    [HttpPost("template-webhook")]
    // [ValidateDocuSealSignature]
    public async Task<IActionResult> HandleTemplateWebhook([FromBody] DocuSealWebhookPayload payload)
    {
        try
        {
            await _docuSealService.TemplateWebhook(payload);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new
            {
                message = "Something went wrong"
            });
        }
    }

    [HttpPost("submission-webhook")]
    // [ValidateDocuSealSignature]
    public async Task<IActionResult> HandleSubmissionWebhook([FromBody] DocuSealWebhookPayload payload)
    {
        _logger.LogInformation("Payload: {JsonPayload}", JsonSerializer.Serialize(payload));

        var validEvents = new[] { "submission.created", "submission.completed", "submission.expired", "submission.archived" };
        if (!validEvents.Contains(payload.EventType))
        {
            return BadRequest(new { message = "Invalid webhook event type" });
        }

        try
        {
            switch (payload.EventType)
            {
                case "submission.created":
                    await _docuSealService.HandleSubmissionCreated(payload.Data);
                    break;
                case "submission.completed":
                    await _docuSealService.HandleSubmissionCompleted(payload.Data);
                    break;
                case "submission.expired":
                    await _docuSealService.HandleSubmissionExpired(payload.Data);
                    break;
                case "submission.archived":
                    await _docuSealService.HandleSubmissionArchived(payload.Data);
                    break;
            }
            return Ok(new { received = true });
        }
        catch (InvalidDataException ex)
        {
            _logger.LogError(ex, "Invalid data format");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Operation failed");
            return NotFound(new { message = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to download document");
            return StatusCode(502, new { message = "Failed to download document from DocuSeal" });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error");
            return StatusCode(500, new { message = "Database operation failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing webhook");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("templates")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<ActionResult> GetAllTemplates()
    {
        try
        {
            var result = await _docuSealService.GetAllTemplates();
            if (!result.IsSuccessful)
            {
                return StatusCode((int)result.StatusCode, result.ErrorMessage);
            }
            return Ok(result.Content);
        }
        catch (Exception ex)
        {
            var errorResponse = new
            {
                message = "Internal server error",
                error = _config.IsDevelopment() ? ex.ToString() : null
            };
            return StatusCode(500, errorResponse);
        }
    }

    [HttpGet("templates/{templateId}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<ActionResult> GetTemplateDetails(string templateId)
    {
        try
        {
            var result = await _docuSealService.GetTemplateDetails(templateId);
            if (!result.IsSuccessful)
            {
                return StatusCode((int)result.StatusCode, result.ErrorMessage);
            }
            return Ok(result.Content);
        }
        catch (Exception ex)
        {
            var errorResponse = new
            {
                message = "Internal server error",
                error = _config.IsDevelopment() ? ex.ToString() : null
            };
            return StatusCode(500, errorResponse);
        }
    }

    [HttpGet("templates/url/{templateId}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<ActionResult> GetTemplateUrl(string templateId)
    {
        try
        {
            var templateDetails = await _docuSealService.GetTemplateDetails(templateId);

            if (!templateDetails.IsSuccessful)
            {
                return StatusCode((int)templateDetails.StatusCode, templateDetails.ErrorMessage);
            }

            if (string.IsNullOrEmpty(templateDetails.Content))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Empty string response from DocuSeal"
                });
            }
            var template = JsonSerializer.Deserialize<JsonElement>(templateDetails.Content);
            var slug = template.GetProperty("slug").GetString();
            var url = $"https://docuseal.com/d/{slug}";
            return Ok(url);
        }
        catch (Exception ex)
        {
            var errorResponse = new
            {
                message = "Internal server error",
                error = _config.IsDevelopment() ? ex.ToString() : null
            };
            return StatusCode(500, errorResponse);
        }
    }

    [HttpPost("templates")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<ActionResult> CreateTemplate(PDFDocument document)
    {
        try
        {
            var result = await _docuSealService.CreateTemplate(document);
            if (!result.IsSuccessful)
            {
                return StatusCode((int)result.StatusCode, result.ErrorMessage);
            }
            return CreatedAtAction(nameof(GetTemplateDetails), new { id = result.TemplateId }, result.TemplateId);
        }
        catch (Exception ex)
        {
            var errorResponse = new
            {
                message = "Internal server error",
                error = _config.IsDevelopment() ? ex.ToString() : null
            };
            return StatusCode(500, errorResponse);
        }
    }
}
