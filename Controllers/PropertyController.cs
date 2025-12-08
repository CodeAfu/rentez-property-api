using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RentEZApi.Services;
using RentEZApi.Exceptions;
using RentEZApi.Models.DTOs.Property;
using RentEZApi.Attributes;
using Microsoft.EntityFrameworkCore;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly ConfigService _config;
    private readonly PropertyService _propertyService;
    private readonly string unknownErrorMessage = "Unknown error occurred";
    private readonly ILogger<PropertyController> _logger;

    public PropertyController(ConfigService config, PropertyService propertyService, ILogger<PropertyController> logger)
    {
        _config = config;
        _propertyService = propertyService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPropertyListings([FromQuery] PropertyFilterRequest filters)
    {
        try
        {
            var result = await _propertyService.GetPaginatedAsync(filters);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Something went wrong"
            });
        }
    }

    [HttpGet("{id}", Name = "GetProperty")]
    // [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProperty(Guid id)
    {
        try
        {
            var property = await _propertyService.Get(id);
            if (property == null)
            {
                return NotFound(new
                {
                    message = "Property does not exist"
                });
            }
            return Ok(property);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = ex.Message,
                message = "Unknown error occurred"
            });
        }
    }

    [HttpGet("u")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetCurrentUserProperty()
    {
        try
        {
            var currentUserClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(currentUserClaim))
            {
                return Unauthorized(new { message = "Unauthorized" });
            }
            var currentUserId = Guid.Parse(currentUserClaim);

            var result = await _propertyService.GetUserOwnedProperty(currentUserId);
            return Ok(result);
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

    [HttpPost]
    [ValidateModelState]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> CreateProperty(CreatePropertyDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized(new
            {
                error = "User authorization check failed",
                message = "Please login to an account to register your property listing"
            });
            var currentUserId = Guid.Parse(userIdClaim);
            var property = await _propertyService.CreateAsync(dto, currentUserId);
            return CreatedAtAction(
                        nameof(GetProperty),
                        new { id = property.Id },
                        property
                    );
        }
        catch (DuplicateObjectException ex)
        {
            return Conflict(new
            {
                error = ex.Message,
                message = "Duplicate property listing detected",
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        error = ex.Message,
                        message = "Unknown Error Occurred",
                    });
        }
    }

    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> EditProperty(Guid id, EditPropertyDto dto)
    {
        try
        {
            var result = await _propertyService.Edit(id, dto);
            return Ok();
        }
        catch (ObjectNotFoundException ex)
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

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<ActionResult> DeleteProperty(Guid id)
    {
        try
        {
            _logger.LogInformation("Starting to delete the Property {id}", id);
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }
            var currentUserId = Guid.Parse(userIdClaim);

            await _propertyService.Delete(id, currentUserId);

            return Ok(new { message = "Property deleted successfully" });
        }
        catch (ObjectNotFoundException ex)
        {
            _logger.LogError(ex, "Object not found");
            return NotFound(new { error = ex.Message, message = "Property Not found" });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error deleting property {PropertyId}", id);
            return Conflict(new
            {
                message = "Cannot delete property due to existing related records"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error occurred {PropertyId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = ex.Message, message = unknownErrorMessage }
            );
        }

    }
}
