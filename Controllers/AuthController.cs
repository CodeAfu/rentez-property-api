using Microsoft.AspNetCore.Mvc;
using RentEZApi.Exceptions;
using RentEZApi.Models.DTOs;
using RentEZApi.Services;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UsersService _userService;
    private readonly string unknownErrorMessage = "Unknown error occurred";

    public AuthController(UsersService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserAuthDto request)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(request.EmailAddress);
            await _userService.VerifyPasswordAsync(user.PasswordHash, request.Password);
            return Accepted(new { message = "Login Successful" });
        }
        catch (UserNotFoundException ex)
        {
            return UserUnauthorized(ex.Message, "Login Failed");
        }
        catch (InvalidPasswordException ex)
        {
            return UserUnauthorized(ex.Message, "Login Failed");
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = ex.Message, message = unknownErrorMessage }
            );
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(CreaterUserDto request)
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
            var createdUser = await _userService.CreateUserAsync(request);
            return Ok(createdUser);
        }
        catch (DuplicateEmailException ex)
        {
            return BadRequest(new { error = ex.Message, message = "Email already exists" });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = ex.Message, message = unknownErrorMessage }
            );
        }
    }

    private UnauthorizedObjectResult UserUnauthorized(string error, string message)
    {
        return Unauthorized(new { error, message });
    }
}