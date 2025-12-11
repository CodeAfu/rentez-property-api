using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;
using RentEZApi.Models.DTOs.PropertyApplication;
using RentEZApi.Models.Entities;
using RentEZApi.Exceptions;

namespace RentEZApi.Services;

public class PropertyApplicationsService
{
    private readonly PropertyDbContext _dbContext;
    private readonly ILogger<PropertyApplicationsService> _logger;

    public PropertyApplicationsService(PropertyDbContext context, ILogger<PropertyApplicationsService> logger)
    {
        _dbContext = context;
        _logger = logger;
    }

    public async Task<PropertyApplicationResponse> CreateAsync(Guid userId, CreateApplicationRequest request)
    {
        // Check if applicant profile exists
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            _logger.LogError($"User not found for ID: {userId}");
            throw new UserNotFoundException($"User not found for ID: {userId}");
        }

        if (user.GovernmentIdNumber == null || user.GovernmentIdType == null)
        {
            _logger.LogError("Please provide your Government ID number and Government ID type to proceed");
            throw new ProfileNotFoundException("Please provide your Government ID number and Government ID type to proceed");
        }

        // Check for duplicate application
        var existingApplication = await _dbContext.PropertyApplications
            .AnyAsync(pa => pa.UserId == user.Id && pa.PropertyId == request.PropertyId);

        if (existingApplication)
            throw new InvalidOperationException("Application already exists for this property");

        var application = new PropertyApplication
        {
            UserId = user.Id,
            PropertyId = request.PropertyId,
            UpdatedAt = DateTime.UtcNow,
        };

        _dbContext.PropertyApplications.Add(application);
        await _dbContext.SaveChangesAsync();

        return await GetApplicationResponse(application.Id);
    }

    public async Task DeleteAsync(Guid applicationId, Guid userId)
    {
        var application = await _dbContext.PropertyApplications
            .Include(u => u.User)
            .FirstOrDefaultAsync(pa => pa.Id == applicationId);

        if (application == null)
            throw new KeyNotFoundException("Application not found");

        if (application.UserId != userId)
            throw new UnauthorizedAccessException("Not authorized to delete this application");

        _dbContext.PropertyApplications.Remove(application);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PropertyApplicationResponse> GetAsync(Guid applicationId, Guid userId)
    {
        var application = await _dbContext.PropertyApplications
            .Include(ap => ap.User)
            .Include(pa => pa.Property)
            .ThenInclude(p => p.Owner)
            .FirstOrDefaultAsync(pa => pa.Id == applicationId);

        if (application == null)
            throw new KeyNotFoundException("Application not found");

        // Check authorization: owner of property or applicant
        var isOwner = application.Property.OwnerId == userId;
        var isApplicant = application.UserId == userId;

        if (!isOwner && !isApplicant)
            throw new UnauthorizedAccessException("Not authorized to view this application");

        return MapToResponse(application);
    }

    public async Task<List<PropertyApplicationResponse>> GetApplicantApplicationsAsync(Guid userId)
    {
        var applications = await _dbContext.PropertyApplications
            .Include(pa => pa.User)
            .Include(pa => pa.Property)
            .ThenInclude(p => p.Owner)
            .Where(pa => pa.UserId == userId)
            .OrderByDescending(pa => pa.CreatedAt)
            .ToListAsync();

        return applications.Select(MapToResponse).ToList();
    }

    public async Task<List<PropertyApplicationResponse>> GetPropertyApplicationsAsync(Guid propertyId, Guid userId)
    {
        var property = await _dbContext.PropertyListings
            .FirstOrDefaultAsync(p => p.Id == propertyId);

        if (property == null)
            throw new KeyNotFoundException("Property not found");

        if (property.OwnerId != userId)
            throw new UnauthorizedAccessException("Not authorized to view applications for this property");

        var applications = await _dbContext.PropertyApplications
            .Include(ap => ap.User)
            .Include(pa => pa.Property)
            .Where(pa => pa.PropertyId == propertyId)
            .OrderByDescending(pa => pa.CreatedAt)
            .ToListAsync();

        return applications.Select(MapToResponse).ToList();
    }

    public async Task<bool> EmailHasBeenSent(string propertyId, string? signerEmail)
    {
        if (signerEmail == null) return false;
        return await _dbContext.PropertyApplications
                .AsNoTracking()
                .Where(s => s.PropertyId == Guid.Parse(propertyId) && s.User.NormalizedEmail == signerEmail.ToUpper())
                .Select(s => s.HasSentEmail)
                .FirstOrDefaultAsync();
    }

    private async Task<PropertyApplicationResponse> GetApplicationResponse(Guid id)
    {
        var application = await _dbContext.PropertyApplications
            .Include(ap => ap.User)
            .Include(pa => pa.Property)
            .FirstAsync(pa => pa.Id == id);

        return MapToResponse(application);
    }

    private PropertyApplicationResponse MapToResponse(PropertyApplication application)
    {
        return new PropertyApplicationResponse
        {
            Id = application.Id,
            PropertyId = application.PropertyId,
            PropertyTitle = application.Property.Title,
            PropertyAddress = application.Property.Address,
            ApplicantUserId = application.UserId,
            ApplicantName = $"{application.User.FirstName} {application.User.LastName}",
            ApplicantEmail = application.User.Email!,
            CreatedAt = application.CreatedAt
        };
    }
}
