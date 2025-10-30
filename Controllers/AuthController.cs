using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEZApi.Exceptions;
using RentEZApi.Models.DTOs.Auth;
using RentEZApi.Models.DTOs.User;
using RentEZApi.Services;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UsersService _usersService;
    private readonly JwtService _jwtService;
    private readonly string unknownErrorMessage = "Unknown error occurred";

    public AuthController(UsersService userService, JwtService jwtService)
    {
        _usersService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _jwtService.Authenticate(request);
            return Ok(response);
        }
        catch (UserNotFoundException ex)
        {
            return UserUnauthorized(ex.Message, "Login Failed");
        }
        catch (InvalidPasswordException ex)
        {
            return UserUnauthorized(ex.Message, "Login Failed");
        }
        catch (UserNotAuthorizedException ex)
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
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
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
            var createdUser = await _usersService.RegisterUserAsync(request);
            return CreatedAtAction(
                nameof(UsersController.GetUser),
                "Users",
                new { id = createdUser.Id },
                createdUser
            );
        }
        catch (DuplicateEmailException ex)
        {
            return BadRequest(new { error = ex.Message, message = "Email already exists" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message, message = "Something went wrong" });
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