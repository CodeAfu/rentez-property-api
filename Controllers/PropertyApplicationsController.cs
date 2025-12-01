using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEZApi.Models.DTOs.PropertyApplication;
using RentEZApi.Services;
using System.Security.Claims;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyApplicationsController : ControllerBase
{
    private readonly PropertyApplicationsService _applicationsService;
    private readonly ILogger<PropertyApplicationsController> _logger;

    public PropertyApplicationsController(
        PropertyApplicationsService applicationsService,
        ILogger<PropertyApplicationsController> logger
    )
    {
        _applicationsService = applicationsService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var application = await _applicationsService.CreateAsync(userId, request);
            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> DeleteApplication(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _applicationsService.DeleteAsync(id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting application");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }

    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetApplication(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var application = await _applicationsService.GetAsync(id, userId);
            return Ok(application);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }

    [HttpGet("my-applications")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetMyApplications()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var applications = await _applicationsService.GetApplicantApplicationsAsync(userId);
            return Ok(applications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applicant applications");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }

    [HttpGet("property/{propertyId}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetPropertyApplications(Guid propertyId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var applications = await _applicationsService.GetPropertyApplicationsAsync(propertyId, userId);
            return Ok(applications);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving property applications");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }
}
