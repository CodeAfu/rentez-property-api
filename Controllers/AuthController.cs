using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;
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
    private readonly PropertyDbContext _dbContext;
    private readonly string unknownErrorMessage = "Unknown error occurred";

    public AuthController(UsersService userService, JwtService jwtService, PropertyDbContext dbContext)
    {
        _usersService = userService;
        _jwtService = jwtService;
        _dbContext = dbContext;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _jwtService.Authenticate(request);

            // Store refresh token in HTTPOnly cookie
            Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // HTTPS only
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

            return Ok(new
            {
                response.Email,
                response.AccessToken,
                response.ExpiresIn
            });
        }
        catch (UserNotFoundException ex)
        {
            return UnauthorizedResponse(ex.Message, "Login Failed");
        }
        catch (InvalidPasswordException ex)
        {
            return UnauthorizedResponse(ex.Message, "Login Failed");
        }
        catch (UnauthorizedException ex)
        {
            return UnauthorizedResponse(ex.Message, "Login Failed");
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

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            // Read refresh token from cookie
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Unauthorized(
                    new {
                        error = "Refresh token missing",
                        message = "User not authorized" 
                    });
            
            var response = await _jwtService.RefreshAccessToken(refreshToken);
            
            // Update cookie with new refresh token
            Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
            
            return Ok(new
            {
                response.Email,
                response.AccessToken,
                response.ExpiresIn
            });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                var hash = RefreshTokenService.HashToken(refreshToken);
                var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == hash);
                if (token != null)
                {
                    token.RevokedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                }
            }
            
            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    private UnauthorizedObjectResult UnauthorizedResponse(string error, string message)
    {
        return Unauthorized(new { error, message });
    }
}