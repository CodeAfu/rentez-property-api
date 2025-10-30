using Microsoft.AspNetCore.Mvc;
using RentEZApi.Models.DTOs.User;
using RentEZApi.Exceptions;
using RentEZApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _userService;
    private readonly ConfigService _configService;
    private readonly string unknownErrorMessage = "Unknown error occurred";

    public UsersController(UsersService userService, ConfigService configService)
    {
        _userService = userService;
        _configService = configService;
    }

    [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        if (users == null) 
            return NotFound(new { message = "Users not found" });
        
        return Ok(users);
    }

    [HttpGet("{id}", Name = "GetUser")]
    [Authorize(Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userService.GetUserAsync(id);
        if (user == null) 
            return NotFound(new { message = "User not found" });
        
        return Ok(user);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateUser(CreateUserDto request)
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
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            var deletedUser = await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { error = ex.Message, message = "User not found"});
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
    [Authorize(Policy = "UserOrAdmin")]
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
            return NotFound(new { error = ex.Message, message = "User not found"});
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