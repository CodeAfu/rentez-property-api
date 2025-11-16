using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RentEZApi.Services;
using RentEZApi.Exceptions;
using RentEZApi.Models.DTOs.Property;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly ConfigService _config;
    private readonly PropertyService _propertyService;
    private readonly string unknownErrorMessage = "Unknown error occurred";

    public PropertyController(ConfigService config, PropertyService propertyService)
    {
        _config = config;
        _propertyService = propertyService;
    }

    [HttpGet]
    // [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetProperties(int pageNum = 1, int lim = 5)
    {
        var result = await _propertyService.GetPaginatedAsync(pageNum, lim);
        return Ok(result);
    }

    [HttpGet("{id}", Name = "GetProperty")]
    // [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetProperty(Guid id)
    {
        try
        {
            var property = await _propertyService.Get(id);
            if (property == null)
            {
                return NotFound(new
                {
                    error = "Property does not exist",
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

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> CreateProperty(CreatePropertyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                error = ModelState
                    .Where(kvp => kvp.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    ),
                message = "Invalid Input"
            });

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
    // [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
    public async Task<IActionResult> EditProperty(Guid id, EditPropertyDto dto)
    {
        try
        {
            // var
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
}
