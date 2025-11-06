using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEZApi.Models.DTOs.DocuSeal;
using RentEZApi.Services;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class DocuSealController : ControllerBase
{
    private readonly DocuSealService _docuSealService;
    private readonly ConfigService _config;

    public DocuSealController(DocuSealService docuSealService, ConfigService config)
    {
        _docuSealService = docuSealService;
        _config = config;
    }


    [HttpPost("builder-token")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public IActionResult GetBuilderToken()
    {
        // var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        // ===== Temp =====
        var userEmail = _config.GetTestEmail();
        if (userEmail == null)
        {
            Console.WriteLine("Environment variable 'TestEmail' is not defined");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    error = "Environment variable 'TestEmail' is not defined",
                    message = "Unknown error occurred",
                });
        }
        // ===== Temp =====
        var tokenString = _docuSealService.GetBuilderToken(userEmail);
        return Ok(new { token = tokenString });
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
    public async Task<ActionResult> GetTemplate(string templateId)
    {
        try
        {
            var result = await _docuSealService.GetTemplate(templateId);
            
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
            return CreatedAtAction(nameof(GetTemplate), new { id = result.TemplateId }, result.TemplateId);
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