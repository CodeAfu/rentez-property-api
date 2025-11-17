using RentEZApi.Data;
using RentEZApi.Models.DTOs.User;
using RentEZApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using RentEZApi.Exceptions;
using Microsoft.AspNetCore.Identity;
using RentEZApi.Models.DTOs.Property;

namespace RentEZApi.Services;

public class UsersService
{
    private readonly PropertyDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public UsersService(PropertyDbContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<List<User>> GetUsersAsync()
            => await _dbContext.Users.ToListAsync();

    public async Task<SelectUserDto?> GetUserAsync(Guid id)
            => await _dbContext.Users
                .Where(u => u.Id == id)
                .Select(u => new SelectUserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Ethnicity = u.Ethnicity,
                    Email = u.Email!,
                    OwnedProperty = u.OwnedProperty.Select(p => new PropertySummaryDto
                    {
                        Title = p.Title,
                        Description = p.Description,
                        Address = p.Address,
                        City = p.City,
                        State = p.State,
                        Rent = p.Rent,
                    }).ToList()
                })
                .FirstOrDefaultAsync();

    public async Task<ICollection<Property>> GetUserProperties(Guid id)
    {
        var user = await _dbContext.Users
                .Include(u => u.OwnedProperty)
                .FirstOrDefaultAsync(u => u.Id == id);
        return user?.OwnedProperty ?? new List<Property>();
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

    public async Task<User> CreateUserAsync(CreateUserDto request)
    {
        if (await CheckEmailExistsAsync(request.Email))
            throw new DuplicateEmailException(request.Email);

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Age = request.Age,
            PhoneNumber = request.PhoneNumber,
            Occupation = request.Occupation,
            Ethnicity = request.Ethnicity,
            PasswordHash = AuthorizationService.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description))
            );
        }

        await _userManager.AddToRoleAsync(user, "User");

        return user;
    }

    public async Task<User> RegisterUserAsync(RegisterUserDto request)
    {
        if (await CheckEmailExistsAsync(request.Email))
            throw new DuplicateEmailException(request.Email);

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            PasswordHash = AuthorizationService.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description))
            );
        }

        await _userManager.AddToRoleAsync(user, "User");
        return user;
    }

    public async Task<User> EditUserAsync(Guid id, EditUserDto request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new UserNotFoundException(id);

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
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
            => await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email) != null;

    public void VerifyPassword(string passwordHash, string password)
    {
        var validationSuccess = AuthorizationService.VerifyPassword(passwordHash, password);
        if (!validationSuccess)
            throw new InvalidPasswordException("Password does not match with database record");
    }
}
