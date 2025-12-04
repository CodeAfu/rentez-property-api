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
    private readonly PropertyApplicationsService _propertyApplicationsService;
    private readonly ILogger<UsersService> _logger;

    public UsersService(
            PropertyDbContext dbContext,
            UserManager<User> userManager,
            ILogger<UsersService> logger,
            PropertyApplicationsService propertyApplicationsService
    )
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _propertyApplicationsService = propertyApplicationsService;
        _logger = logger;
    }

    public async Task<List<User>> GetUsersAsync()
            => await _dbContext.Users.ToListAsync();

    public async Task<SelectUserDto?> GetUserAsync(Guid id)
            => await _dbContext.Users
                .Where(u => u.Id == id)
                .Select(u => new SelectUserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DateOfBirth = u.DateOfBirth,
                    Ethnicity = u.Ethnicity,
                    Occupation = u.Occupation,
                    Email = u.Email!,
                    MonthlyIncome = u.MonthlyIncome,
                    EmployerName = u.EmployerName,
                    GovernmentIdType = u.GovernmentIdType,
                    GovernmentIdNumber = u.GovernmentIdNumber,
                    NumberOfOccupants = u.NumberOfOccupants,
                    HasPets = u.HasPets,
                    PetDetails = u.PetDetails,
                    OwnedProperty = u.OwnedProperty.Select(p => new PropertySummaryDto
                    {
                        Title = p.Title,
                        Description = p.Description!,
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
            FirstName = request.FirstName!,
            LastName = request.LastName!,
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = request.PhoneNumber,
            Occupation = request.Occupation!,
            Ethnicity = request.Ethnicity!,
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

        if (request.DateOfBirth != null)
            user.DateOfBirth = request.DateOfBirth;

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;

        if (!string.IsNullOrWhiteSpace(request.Occupation))
            user.Occupation = request.Occupation;

        if (!string.IsNullOrWhiteSpace(request.Ethnicity))
            user.Ethnicity = request.Ethnicity;

        // Application Profile fields
        if (request.MonthlyIncome.HasValue)
            user.MonthlyIncome = request.MonthlyIncome;

        if (!string.IsNullOrWhiteSpace(request.EmployerName))
            user.EmployerName = request.EmployerName;

        if (!string.IsNullOrWhiteSpace(request.GovernmentIdType))
            user.GovernmentIdType = request.GovernmentIdType;

        if (!string.IsNullOrWhiteSpace(request.GovernmentIdNumber))
            user.GovernmentIdNumber = request.GovernmentIdNumber;

        if (request.NumberOfOccupants.HasValue)
            user.NumberOfOccupants = request.NumberOfOccupants;

        if (request.HasPets.HasValue)
            user.HasPets = request.HasPets;

        if (!string.IsNullOrWhiteSpace(request.PetDetails))
            user.PetDetails = request.PetDetails;

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

    public async Task<ApplicantProfileResponse> CreateProfileAsync(Guid userId, CreateApplicantProfileRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new KeyNotFoundException("User does not exist");

        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.GovernmentIdType) ||
            string.IsNullOrWhiteSpace(request.GovernmentIdNumber))
            throw new InvalidOperationException("Government ID information is required");

        user.MonthlyIncome = request.MonthlyIncome;
        user.EmployerName = request.EmployerName;
        user.GovernmentIdType = request.GovernmentIdType;
        user.GovernmentIdNumber = request.GovernmentIdNumber;
        user.NumberOfOccupants = request.NumberOfOccupants;
        user.HasPets = request.HasPets;
        user.PetDetails = request.PetDetails;

        await _dbContext.SaveChangesAsync();

        return new ApplicantProfileResponse
        {
            MonthlyIncome = user.MonthlyIncome,
            EmployerName = user.EmployerName,
            GovernmentIdType = user.GovernmentIdType,
            GovernmentIdNumber = user.GovernmentIdNumber,
            NumberOfOccupants = user.NumberOfOccupants,
            HasPets = user.HasPets,
            PetDetails = user.PetDetails,
        };
    }

    public async Task<PropertyApplication> SendRentPropertyRequest(Guid userId, Guid propertyId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            _logger.LogError($"User not found for ID: {userId}");
            throw new UserNotFoundException($"User not found for ID: {userId}");
        }

        if (string.IsNullOrWhiteSpace(user.GovernmentIdNumber) ||
            string.IsNullOrWhiteSpace(user.GovernmentIdType))
        {
            _logger.LogError("Please provide your Government ID number and Government ID type to proceed");
            throw new ProfileNotFoundException("Please provide your Government ID number and Government ID type to proceed");
        }

        var propertyExists = await _dbContext.Property
            .AnyAsync(p => p.Id == propertyId);

        if (!propertyExists)
            throw new ObjectNotFoundException($"Property {propertyId} not found");

        // Check for duplicate application
        var existingApplication = await _dbContext.PropertyApplications
            .AnyAsync(pa => pa.UserId == userId && pa.PropertyId == propertyId);

        if (existingApplication)
            throw new InvalidOperationException("You have already applied to this property");

        var application = new PropertyApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PropertyId = propertyId
        };

        _dbContext.PropertyApplications.Add(application);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("User {UserId} applied to property {PropertyId}", userId, propertyId);

        return application;
    }
}
