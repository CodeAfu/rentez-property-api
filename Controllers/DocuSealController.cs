using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult GetBuilderToken()
    {
        // var userEmail = _config.GetTestEmail();
        var adminEmail = _config.GetProdEmail();
        _logger.LogInformation("Using Production Email: ", adminEmail);
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var tokenString = _docuSealService.GetBuilderToken(adminEmail);
        return Ok(new { token = tokenString });
    }

    [HttpPost("template-webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleTemplateWebhook([FromBody] DocuSealWebhookPayload payload)
    {
        var webhookSecret = _config.GetWebhookSecret();
        var signature = Request.Headers["X-Docuseal-Signature"].FirstOrDefault();

        await _docuSealService.TemplateWebhook(payload);

        return Ok();
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
