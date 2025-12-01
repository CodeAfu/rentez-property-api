using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;
using RentEZApi.Models.DTOs.PropertyApplication;
using RentEZApi.Models.Entities;

namespace RentEZApi.Services;

public class PropertyApplicationsService
{
    private readonly PropertyDbContext _dbContext;

    public PropertyApplicationsService(PropertyDbContext context)
    {
        _dbContext = context;
    }

    public async Task<PropertyApplicationResponse> CreateAsync(Guid userId, CreateApplicationRequest request)
    {
        // Check if applicant profile exists
        var profile = await _dbContext.ApplicantProfiles
            .FirstOrDefaultAsync(ap => ap.UserId == userId);

        if (profile == null)
            throw new InvalidOperationException("Applicant profile must be created before applying");

        // Check for duplicate application
        var existingApplication = await _dbContext.PropertyApplications
            .AnyAsync(pa => pa.ApplicantId == profile.Id && pa.PropertyId == request.PropertyId);

        if (existingApplication)
            throw new InvalidOperationException("Application already exists for this property");

        var application = new PropertyApplication
        {
            ApplicantId = profile.Id,
            PropertyId = request.PropertyId
        };

        _dbContext.PropertyApplications.Add(application);
        await _dbContext.SaveChangesAsync();

        return await GetApplicationResponse(application.Id);
    }

    public async Task DeleteAsync(Guid applicationId, Guid userId)
    {
        var application = await _dbContext.PropertyApplications
            .Include(pa => pa.ApplicantProfile)
            .FirstOrDefaultAsync(pa => pa.Id == applicationId);

        if (application == null)
            throw new KeyNotFoundException("Application not found");

        if (application.ApplicantProfile.UserId != userId)
            throw new UnauthorizedAccessException("Not authorized to delete this application");

        _dbContext.PropertyApplications.Remove(application);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PropertyApplicationResponse> GetAsync(Guid applicationId, Guid userId)
    {
        var application = await _dbContext.PropertyApplications
            .Include(pa => pa.ApplicantProfile)
            .ThenInclude(ap => ap.User)
            .Include(pa => pa.Property)
            .ThenInclude(p => p.Owner)
            .FirstOrDefaultAsync(pa => pa.Id == applicationId);

        if (application == null)
            throw new KeyNotFoundException("Application not found");

        // Check authorization: owner of property or applicant
        var isOwner = application.Property.OwnerId == userId;
        var isApplicant = application.ApplicantProfile.UserId == userId;

        if (!isOwner && !isApplicant)
            throw new UnauthorizedAccessException("Not authorized to view this application");

        return MapToResponse(application);
    }

    public async Task<List<PropertyApplicationResponse>> GetApplicantApplicationsAsync(Guid userId)
    {
        var applications = await _dbContext.PropertyApplications
            .Include(pa => pa.ApplicantProfile)
            .Include(pa => pa.Property)
            .ThenInclude(p => p.Owner)
            .Where(pa => pa.ApplicantProfile.UserId == userId)
            .OrderByDescending(pa => pa.CreatedAt)
            .ToListAsync();

        return applications.Select(MapToResponse).ToList();
    }

    public async Task<List<PropertyApplicationResponse>> GetPropertyApplicationsAsync(Guid propertyId, Guid userId)
    {
        var property = await _dbContext.Property
            .FirstOrDefaultAsync(p => p.Id == propertyId);

        if (property == null)
            throw new KeyNotFoundException("Property not found");

        if (property.OwnerId != userId)
            throw new UnauthorizedAccessException("Not authorized to view applications for this property");

        var applications = await _dbContext.PropertyApplications
            .Include(pa => pa.ApplicantProfile)
            .ThenInclude(ap => ap.User)
            .Include(pa => pa.Property)
            .Where(pa => pa.PropertyId == propertyId)
            .OrderByDescending(pa => pa.CreatedAt)
            .ToListAsync();

        return applications.Select(MapToResponse).ToList();
    }

    private async Task<PropertyApplicationResponse> GetApplicationResponse(Guid id)
    {
        var application = await _dbContext.PropertyApplications
            .Include(pa => pa.ApplicantProfile)
            .ThenInclude(ap => ap.User)
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
            ApplicantId = application.ApplicantId,
            ApplicantName = $"{application.ApplicantProfile.User.FirstName} {application.ApplicantProfile.User.LastName}",
            ApplicantEmail = application.ApplicantProfile.User.Email!,
            CreatedAt = application.CreatedAt
        };
    }
}
