using Microsoft.AspNetCore.Mvc;
using RentEZApi.Models.DTOs.User;
using RentEZApi.Exceptions;
using RentEZApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RentEZApi.Attributes;
using RentEZApi.Models.DTOs.ApplicantProfile;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _userService;
    private readonly ConfigService _configService;
    private readonly ApplicantProfilesService _applicantProfilesService;
    private readonly string unknownErrorMessage = "Unknown error occurred";
    private readonly ILogger<UsersController> _logger;

    public UsersController(UsersService userService, ConfigService configService, ILogger<UsersController> logger, ApplicantProfilesService applicantProfileService)
    {
        _userService = userService;
        _configService = configService;
        _logger = logger;
        _applicantProfilesService = applicantProfileService;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminOnly")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        if (users == null)
            return NotFound(new { message = "Users not found" });
        return Ok(users);
    }

    [HttpGet("{id}", Name = "GetUser")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userService.GetUserAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });
        return Ok(user);
    }

    [HttpGet("u")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var currentUserClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserClaim == null)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        var currentUserId = Guid.Parse(currentUserClaim);
        var user = await _userService.GetUserAsync(currentUserId);

        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(user);
    }

    [HttpGet("u/applicant-profile")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetCurrentUserApplicantProfile()
    {
        var currentUserClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserClaim == null)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }
        var currentUserId = Guid.Parse(currentUserClaim);

        var profile = await _applicantProfilesService.GetByUserIdAsync(currentUserId);

        if (profile == null)
            return NotFound(new { message = "Applicant profile not found" });

        return Ok(profile);
    }

    [HttpPut("u/applicant-profile/edit")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> UpdateCurrentUserApplicantProfile([FromBody] UpdateApplicantProfileRequest request)
    {
        var currentUserClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserClaim == null)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }
        var currentUserId = Guid.Parse(currentUserClaim);

        try
        {
            var profile = await _applicantProfilesService.GetByUserIdAsync(currentUserId);

            if (profile == null)
                return NotFound(new { message = "Applicant profile not found" });

            var updated = await _applicantProfilesService.UpdateAsync(profile.Id, currentUserId, request);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating applicant profile");
            return StatusCode(500, new { message = "Something went wrong" });
        }
    }

    [HttpGet("property/{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetUserProperty(Guid id)
    {
        var property = await _userService.GetUserProperties(id);
        if (property == null)
            return NotFound(new { message = "User has no owned property listings" });
        return Ok(property);
    }

    [HttpPost]
    [ValidateModelState]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminOnly")]
    public async Task<IActionResult> CreateUser(CreateUserDto request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(
                nameof(GetUser),
                new { id = user.Id },
                user
            );
        }
        catch (DuplicateEmailException ex)
        {
            return Conflict(new { error = ex.Message, message = "Email address is already taken" });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = ex.Message, message = unknownErrorMessage }
            );
        }
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            var deletedUser = await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { error = ex.Message, message = "User not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = ex.Message, message = unknownErrorMessage }
            );
        }
    }

    [HttpPut("{id}")]
    [ValidateModelState]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> EditUser(Guid id, EditUserDto request)
    {
        try
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && currentUserId != id.ToString())
            {
                return Forbid();
            }
            var edittedUser = await _userService.EditUserAsync(id, request);
            return Ok(edittedUser);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { error = ex.Message, message = "User not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = ex.Message, message = unknownErrorMessage }
            );
        }
    }
}
