using RentEZApi.Data;
using RentEZApi.Models.DTOs;
using RentEZApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using RentEZApi.Models.Response;
using Models.DTOs;

namespace RentEZApi.Services;

public class UserService
{
    private readonly PropertyDbContext _dbContext;
    public UserService(PropertyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<User>>> GetUsersAsync()
    {
        var users = await _dbContext.Users.ToListAsync();
        return Result<List<User>>.Success(users, $"Fetched {users.Count} users");
    }

    public async Task<Result<User>> GetUserAsync(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return Result<User>.Failure($"User with '{id}' not found", "User not found");
        return Result<User>.Success(user, "User found");
    }

    public async Task<Result<User>> GetUserByEmailAsync(string emailAddress)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailAddress == emailAddress);
        if (user == null) return Result<User>.Failure("Invalid email or password", "Invalid Email or Password");

        return Result<User>.Success(user, "User found");
    }

    public async Task<Result<User>> DeleteUserAsync(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return Result<User>.Failure($"User with '{id}' not found", "User not found");

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return Result<User>.Success(user, $"User '{user.FirstName} {user.LastName}' deleted successfully");
    }

    public async Task<Result<User>> CreateUserAsync(CreaterUserDto request)
    {
        var emailExists = await EmailExistsAsync(request.EmailAddress);
        if (emailExists)
            return Result<User>.Failure($"Email Address '{request.EmailAddress}' already exists", "A user with this email already");

        var hashedPassword = AuthorizationService.HashPassword(request.Password);
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Age = request.Age,
            PhoneNumber = request.PhoneNumber,
            Occupation = request.Occupation,
            Ethnicity = request.Ethnicity,
            EmailAddress = request.EmailAddress,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return Result<User>.Success(user, "User created successfully");
    }

    public async Task<Result<User>> EditUserAsync(Guid id, EditUserDto request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        // TODO: Add Age Validation
        
        if (user == null)
            return Result<User>.Failure($"User with id '{id}' not found", "User not found");

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.LastName = request.LastName;

        if (request.Age > 0)
            user.Age = request.Age;

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;

        if (!string.IsNullOrWhiteSpace(request.Occupation))
            user.Occupation = request.Occupation;

        if (!string.IsNullOrWhiteSpace(request.Ethnicity))
            user.Ethnicity = request.Ethnicity;

        user.UpdatedAt = DateTime.UtcNow;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return Result<User>.Success(user, "User editted succesfully");
    }

    public async Task<bool> EmailExistsAsync(string emailAddress)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailAddress == emailAddress);
        return existingUser != null;
    }

    public async Task<Result<bool>> VerifyPasswordAsync(string passwordHash, string password)
    {
        var validationSuccess = AuthorizationService.VerifyPassword(passwordHash, password);
        if (!validationSuccess) return Result<bool>.Failure("Invalid email or password", "Invalid Email or Password");

        return Result<bool>.Success(validationSuccess, "Validation successful");
    }
}