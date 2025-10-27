using RentEZApi.Data;
using RentEZApi.Models.DTOs;
using RentEZApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using RentEZApi.Exceptions;

namespace RentEZApi.Services;

public class UsersService
{
    private readonly PropertyDbContext _dbContext;

    public UsersService(PropertyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<User>> GetUsersAsync() 
            => await _dbContext.Users.ToListAsync();

    public async Task<User?> GetUserAsync(Guid id) 
            => await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User> GetUserByEmailAsync(string emailAddress)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailAddress == emailAddress);
        if (user == null)
            throw new UserNotFoundException($"User with email '{emailAddress} not found");

        return user;
    }

    public async Task<User> DeleteUserAsync(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new UserNotFoundException(id);

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User> CreateUserAsync(CreaterUserDto request)
    {
        if (await EmailExistsAsync(request.EmailAddress))
            throw new DuplicateEmailException(request.EmailAddress);
            
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Age = request.Age,
            PhoneNumber = request.PhoneNumber,
            Occupation = request.Occupation,
            Ethnicity = request.Ethnicity,
            EmailAddress = request.EmailAddress,
            PasswordHash = AuthorizationService.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> EditUserAsync(Guid id, EditUserDto request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new UserNotFoundException(id);

        // TODO: Add Age Validation

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

        return user;
    }

    public async Task<bool> EmailExistsAsync(string emailAddress)
            => await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailAddress == emailAddress) != null;

    public async Task VerifyPasswordAsync(string passwordHash, string password)
    {
        var validationSuccess = AuthorizationService.VerifyPassword(passwordHash, password);
        if (!validationSuccess)
            throw new InvalidPasswordException("Password does not match with database record");
    }
}