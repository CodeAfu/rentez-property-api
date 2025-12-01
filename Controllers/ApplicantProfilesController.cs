using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEZApi.Models.DTOs.ApplicantProfile;
using RentEZApi.Services;
using System.Security.Claims;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicantProfilesController : ControllerBase
{
    private readonly ApplicantProfilesService _profilesService;
    private readonly ILogger<ApplicantProfilesController> _logger;

    public ApplicantProfilesController(
        ApplicantProfilesService profilesService,
        ILogger<ApplicantProfilesController> logger
    )
    {
        _profilesService = profilesService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> CreateProfile([FromBody] CreateApplicantProfileRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _profilesService.CreateAsync(userId, request);
            return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating applicant profile");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateApplicantProfileRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _profilesService.UpdateAsync(id, userId, request);
            return Ok(profile);
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
            _logger.LogError(ex, "Error updating applicant profile");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }

    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _profilesService.GetAsync(id, userId);
            return Ok(profile);
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
            _logger.LogError(ex, "Error retrieving applicant profile");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }

    [HttpGet("my-profile")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _profilesService.GetByUserIdAsync(userId);
            if (profile == null)
                return NotFound(new { message = "Profile not found" });

            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> DeleteProfile(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _profilesService.DeleteAsync(id, userId);
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
            _logger.LogError(ex, "Error deleting applicant profile");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }
}
