using System.Security.Claims;
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
    public async Task<IActionResult> GetBuilderToken()
    {
        // var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var userEmail = "ibrahim.afzal1999+test@gmail.com";
        var tokenString = _docuSealService.GetBuilderToken(userEmail);
        return Ok(new { token = tokenString });
    }

    [HttpGet("templates")]
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