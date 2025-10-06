using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using RentEZApi.Data;
using RentEZApi.Models.DTOs;
using RentEZApi.Models.Entities;
using RentEZApi.Models.Response;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly PropertyDbContext _dbContext;

    public UserController(PropertyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult> GetUsers()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment != "Development")
        {
            return BadRequest(
                ApiResponse.Fail("You do not have permission to access this endpoint")
            );
        }

        var users = await _dbContext.Users.ToListAsync();

        return Ok(
            ApiResponse<List<User>>.FromData(users, $"{users.Count} Users found")
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser(Guid id)
    {
        var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound(
                ApiResponse.Fail($"User with id '{id}' not found.")
            );

        return Ok(ApiResponse<User>.FromData(user, "User Found Successfully"));
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreaterUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail("Invalid input", ModelState.ToString()));

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Age = dto.Age,
            PhoneNumber = dto.PhoneNumber,
            Occupation = dto.Occupation,
            Ethnicity = dto.Ethnicity,
            EmailAddress = dto.EmailAddress,
            PasswordHash = dto.PasswordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id },
            ApiResponse<User>.FromData(user, "User created successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound(ApiResponse.Fail($"User with id '{id}' not found"));

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return Ok(ApiResponse<User>.FromData(user, $"User '{user.FirstName} {user.LastName}' deleted successfully"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> EditUser(Guid id, EditUserDto dto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound(ApiResponse.Fail($"User with id '{id}' not found"));

        // Update only non-null fields (partial update)
        if (!string.IsNullOrWhiteSpace(dto.FirstName))
            user.FirstName = dto.FirstName;

        if (!string.IsNullOrWhiteSpace(dto.LastName))
            user.LastName = dto.LastName;

        if (dto.Age > 0)
            user.Age = dto.Age;

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            user.PhoneNumber = dto.PhoneNumber;

        if (!string.IsNullOrWhiteSpace(dto.Occupation))
            user.Occupation = dto.Occupation;

        if (!string.IsNullOrWhiteSpace(dto.Ethnicity))
            user.Ethnicity = dto.Ethnicity;

        user.UpdatedAt = DateTime.UtcNow;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return Ok(ApiResponse<User>.FromData(user, "User updated successfully"));
    }
}