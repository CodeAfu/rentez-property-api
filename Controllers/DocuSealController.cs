using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEZApi.Models.DTOs.DocuSeal.Submission;
using RentEZApi.Models.DTOs.DocuSeal.Template;
using RentEZApi.Services;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocuSealController : ControllerBase
{
    private readonly DocuSealService _docuSealService;
    private readonly PropertyService _propertyService;
    private readonly PropertyApplicationsService _propertyApplicationsService;
    private readonly DocuSealSubmissionsService _docuSealSubmissionsService;
    private readonly ConfigService _config;
    private readonly ILogger<DocuSealController> _logger;

    public DocuSealController(
            DocuSealService docuSealService,
            PropertyService propertyService,
            PropertyApplicationsService propertyApplicationsService,
            DocuSealSubmissionsService docuSealSubmissionsService,
            ConfigService config,
            ILogger<DocuSealController> logger
    )
    {
        _docuSealService = docuSealService;
        _propertyService = propertyService;
        _propertyApplicationsService = propertyApplicationsService;
        _docuSealSubmissionsService = docuSealSubmissionsService;
        _config = config;
        _logger = logger;
    }

    [HttpPost("builder-token")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetBuilderToken([FromQuery] string propertyId, [FromQuery] string? templateId, [FromQuery] string? signerEmail)
    {
        // var adminEmail = _config.GetTestEmail();
        var adminEmail = _config.GetProdEmail();
        if (string.IsNullOrEmpty(adminEmail))
        {
            _logger.LogError("Please setup a 'ProdEmail` environment variable with your registered DocuSeal email");
            return Conflict(new
            {
                message = "Unknown error occurred"
            });
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized(new
            {
                message = "Please login to use this feature"
            });

        try
        {
            var tokenString = await _docuSealService.GetBuilderToken(adminEmail, currentUserId, propertyId, templateId);
            var owner = await _propertyService.GetPropertyOwner(Guid.Parse(propertyId));
            var emailSent = await _propertyApplicationsService.EmailHasBeenSent(propertyId, signerEmail);
            return Ok(new
            {
                token = tokenString,
                signerEmail = signerEmail,
                emailSent = emailSent,
                ownerName = owner != null ? (owner?.FirstName + " " + owner?.LastName).Trim() : null
            });
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

    [HttpPost("signer-token")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetSignerToken([FromQuery] string slug, [FromQuery] Guid? propertyId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized(new { message = "Please login to use this feature" });

        try
        {
            var tokenString = _docuSealService.GetSignerToken(slug, propertyId);
            return Ok(new { token = tokenString });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation");
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while getting signer token");
            return StatusCode(500, new
            {
                message = "Something went wrong"
            });
        }
    }

    [HttpPost("save-lease")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> SaveTemplateData([FromQuery] Guid propertyId, [FromQuery] Guid templateId, [FromBody] TemplatePayloadDto payload)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var response = await _docuSealService.SaveDocuSealTemplate(propertyId, templateId, userId, payload);
            return Ok(new
            {
                message = "Template saved successfully",
                data = response,
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid input");
            return BadRequest(new
            {
                message = $"Invalid input: {ex.Message}"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "User Unauthorized");
            return Unauthorized(new
            {
                message = "You are not authorized to do this action"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong");
            return StatusCode(500, new
            {
                message = "Something went wrong"
            });
        }
    }

    [HttpPost("create-lease")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> SaveTemplateData([FromQuery] Guid propertyId, [FromQuery] string? signerEmail, [FromBody] TemplatePayloadDto payload)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var response = await _docuSealService.CreateDocuSealTemplate(userId, propertyId, payload);
            return Ok(new
            {
                created = response.Created,
                templateId = response.TemplateId,
                message = response.Created ? "Template created successfully" : "Template already exists",
                signerEmail = signerEmail,
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid input");
            return BadRequest(new
            {
                message = $"Invalid input: {ex.Message}"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "User Unauthorized");
            return Unauthorized(new
            {
                message = "You are not authorized to do this action"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong");
            return StatusCode(500, new
            {
                message = "Something went wrong"
            });
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
            _logger.LogError(ex, "Somewthing went wrong");
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
            _logger.LogError(ex, "Unknown error occurred");
            var errorResponse = new
            {
                message = "Internal server error",
                error = _config.IsDevelopment() ? ex.ToString() : null
            };
            return StatusCode(500, errorResponse);
        }
    }

    [HttpPost("submissions")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<ActionResult> CreateSubmission([FromBody] CreateSubmissionRequestDto dto)
    {
        try
        {
            var result = await _docuSealSubmissionsService.CreateSubmission(dto);
            return StatusCode(201, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Operation error");
            return BadRequest(new { message = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error while fetching from DocuSeal API");
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = "Unable to process lease agreement at this time. Please try again later.",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown error occurred");
            return StatusCode(500, new
            {
                message = "Unknown error occurred. See logs for more information"
            });
        }
    }

    [HttpGet("submissions")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<ActionResult> GetSubmissionsByTemplateId([FromQuery] string propertyId)
    {
        try
        {
            var result = await _docuSealSubmissionsService.GetSubmissionsByPropertyId(propertyId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Operation error");
            return BadRequest(new { message = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error while fetching from DocuSeal API");
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = "Unable to process lease agreement at this time. Please try again later.",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown error occurred");
            return StatusCode(500, new
            {
                message = "Unknown error occurred. See logs for more information"
            });
        }
    }

    // [HttpPost("templates")]
    // [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    // public async Task<ActionResult> CreateTemplate(PDFDocument document)
    // {
    //     try
    //     {
    //         var result = await _docuSealService.CreateTemplate(document);
    //         if (!result.IsSuccessful)
    //         {
    //             return StatusCode((int)result.StatusCode, result.ErrorMessage);
    //         }
    //         return CreatedAtAction(nameof(GetTemplateDetails), new { id = result.TemplateId }, result.TemplateId);
    //     }
    //     catch (Exception ex)
    //     {
    //         var errorResponse = new
    //         {
    //             message = "Internal server error",
    //             error = _config.IsDevelopment() ? ex.ToString() : null
    //         };
    //         return StatusCode(500, errorResponse);
    //     }
    // }
}
