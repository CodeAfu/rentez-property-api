using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using RentEZApi.Models.DTOs;
using RentEZApi.Models.Entities;
using RentEZApi.Models.Response;
using RentEZApi.Services;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult> GetUsers()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment != "Development")
        {
            return BadRequest(
                ApiResponse.Fail("Invalid Permission", "You do not have permission to access this endpoint")
            );
        }

        var usersResult = await _userService.GetUsersAsync();

        if (usersResult.Data == null) return InternalServerError("usersResult Data is null");

        return Ok(
            ApiResponse<List<User>>.FromData(usersResult.Data, usersResult.Message)
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser(Guid id)
    {
        var userResult = await _userService.GetUserAsync(id);

        if (userResult.Data == null)
            return NotFound(
                ApiResponse.Fail("User not found", $"User with id '{id}' not found.")
            );

        return Ok(ApiResponse<User>.FromData(userResult.Data, userResult.Message));
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreaterUserDto request)
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

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var deleteUserResult = await _userService.DeleteUserAsync(id);

        if (deleteUserResult.Error != null) 
            return NotFound(ApiResponse.Fail(deleteUserResult.Error, deleteUserResult.Message));

        if (deleteUserResult.Data == null) return InternalServerError("User is null");

        return Ok(ApiResponse<User>
            .FromData(deleteUserResult.Data, deleteUserResult.Message));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> EditUser(Guid id, EditUserDto request)
    {
        var editUserResult = await _userService.EditUserAsync(id, request);

        if (editUserResult.Error != null)
            return NotFound(ApiResponse<User>.Fail(editUserResult.Error, editUserResult.Message));

        if (editUserResult.Data == null) return InternalServerError("editUserResult Data is null");

        return Ok(ApiResponse<User>.FromData(editUserResult.Data, editUserResult.Message));
    }

    private ActionResult InternalServerError(string? message)
    {
        return StatusCode(StatusCodes.Status500InternalServerError,
            ApiResponse.Fail("Internal Server Error", message));
    }
}