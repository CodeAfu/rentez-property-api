using Microsoft.AspNetCore.Mvc;
using RentEZApi.Models.DTOs;
using RentEZApi.Models.Entities;
using RentEZApi.Models.Response;
using RentEZApi.Services;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserAuthDto request)
    {
        var userResult = await _userService.GetUserByEmailAsync(request.EmailAddress);

        if (userResult.Error != null)
            return Unauthorized(ApiResponse.Fail(userResult.Error, "Login Failed"));

        if (userResult.Data == null) return InternalServerError("User is null");

        var validationResult = await _userService.VerifyPasswordAsync(userResult.Data.PasswordHash, request.Password);

        if (validationResult.Error != null) 
            return Unauthorized(ApiResponse.Fail(validationResult.Error, "Login Failed"));

        if (validationResult.Data == false)
            return Unauthorized(ApiResponse.Fail("Login failed", "You are not authorized to perform this action"));

        return Ok(ApiResponse<UserAuthDto>.FromData(request, "Login Successful"));
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(CreaterUserDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail("Invalid input", ModelState.ToString()));

        var createUserResult = await _userService.CreateUserAsync(request);

        if (createUserResult.Error != null)
            return Conflict(ApiResponse.Fail(createUserResult.Error, createUserResult.Message));

        if (createUserResult.Data == null)
            return InternalServerError("User is null");

        return Ok(ApiResponse<User>.FromData(createUserResult.Data, createUserResult.Message));
    }

    private ActionResult InternalServerError(string? message)
    {
        return StatusCode(StatusCodes.Status500InternalServerError,
            ApiResponse.Fail("Internal Server Error", message));
    }
}