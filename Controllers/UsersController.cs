using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using RentEZApi.Exceptions;
using RentEZApi.Models.DTOs;
using RentEZApi.Services;

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

    [HttpGet]
    public async Task<ActionResult> GetUsers()
    {
        if (_configService.GetEnvironment() != "Development")
        {
            return Unauthorized(
                new
                {
                    error = "User does not have permissions to access this endpoint",
                    message = "Not allowed",
                }
            );
        }

        var user = await _userService.GetUsersAsync();
        if (user == null) 
            return NotFound(new { message = "User not found" });
        
        return Ok(user);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser(Guid id)
    {
        var user = await _userService.GetUserAsync(id);
        if (user == null) 
            return NotFound(new { message = "User not found" });
        
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreaterUserDto request)
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
    public async Task<ActionResult> DeleteUser(Guid id)
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
    public async Task<ActionResult> EditUser(Guid id, EditUserDto request)
    {
        try
        {
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