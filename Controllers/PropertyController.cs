using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEZApi.Services;
using RentEZApi.Exceptions;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly ConfigService _config;
    private readonly PropertyService _propertyService;

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
    // [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
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
            var property = await _propertyService.CreateAsync(dto);
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
}
