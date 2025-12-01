using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;
using RentEZApi.Models.DTOs.ApplicantProfile;
using RentEZApi.Models.Entities;

namespace RentEZApi.Services;

public class ApplicantProfilesService
{
    private readonly PropertyDbContext _dbContext;

    public ApplicantProfilesService(PropertyDbContext context)
    {
        _dbContext = context;
    }

    public async Task<ApplicantProfileResponse> CreateAsync(Guid userId, CreateApplicantProfileRequest request)
    {
        var existingProfile = await _dbContext.ApplicantProfiles
            .AnyAsync(ap => ap.UserId == userId);

        if (existingProfile)
            throw new InvalidOperationException("Applicant profile already exists");

        var profile = new ApplicantProfile
        {
            UserId = userId,
            MonthlyIncome = request.MonthlyIncome,
            EmployerName = request.EmployerName,
            GovernmentIdType = request.GovernmentIdType,
            GovernmentIdNumber = request.GovernmentIdNumber,
            NumberOfOccupants = request.NumberOfOccupants,
            HasPets = request.HasPets,
            PetDetails = request.PetDetails
        };

        _dbContext.ApplicantProfiles.Add(profile);
        await _dbContext.SaveChangesAsync();

        return await GetProfileResponse(profile.Id);
    }

    public async Task<ApplicantProfileResponse> UpdateAsync(Guid profileId, Guid userId, UpdateApplicantProfileRequest request)
    {
        var profile = await _dbContext.ApplicantProfiles
            .FirstOrDefaultAsync(ap => ap.Id == profileId);

        if (profile == null)
            throw new KeyNotFoundException("Profile not found");

        if (profile.UserId != userId)
            throw new UnauthorizedAccessException("Not authorized to update this profile");

        profile.MonthlyIncome = request.MonthlyIncome ?? profile.MonthlyIncome;
        profile.EmployerName = request.EmployerName ?? profile.EmployerName;
        profile.GovernmentIdType = request.GovernmentIdType ?? profile.GovernmentIdType;
        profile.GovernmentIdNumber = request.GovernmentIdNumber ?? profile.GovernmentIdNumber;
        profile.NumberOfOccupants = request.NumberOfOccupants ?? profile.NumberOfOccupants;
        profile.HasPets = request.HasPets ?? profile.HasPets;
        profile.PetDetails = request.PetDetails ?? profile.PetDetails;
        profile.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetProfileResponse(profileId);
    }

    public async Task<ApplicantProfileResponse> GetAsync(Guid profileId, Guid userId)
    {
        var profile = await _dbContext.ApplicantProfiles
            .Include(ap => ap.User)
            .FirstOrDefaultAsync(ap => ap.Id == profileId);

        if (profile == null)
            throw new KeyNotFoundException("Profile not found");

        // Only owner or admin can view
        if (profile.UserId != userId)
            throw new UnauthorizedAccessException("Not authorized to view this profile");

        return MapToResponse(profile);
    }

    public async Task<ApplicantProfileResponse?> GetByUserIdAsync(Guid userId)
    {
        var profile = await _dbContext.ApplicantProfiles
            .Include(ap => ap.User)
            .FirstOrDefaultAsync(ap => ap.UserId == userId);

        return profile == null ? null : MapToResponse(profile);
    }

    public async Task DeleteAsync(Guid profileId, Guid userId)
    {
        var profile = await _dbContext.ApplicantProfiles
            .FirstOrDefaultAsync(ap => ap.Id == profileId);

        if (profile == null)
            throw new KeyNotFoundException("Profile not found");

        if (profile.UserId != userId)
            throw new UnauthorizedAccessException("Not authorized to delete this profile");

        _dbContext.ApplicantProfiles.Remove(profile);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<ApplicantProfileResponse> GetProfileResponse(Guid id)
    {
        var profile = await _dbContext.ApplicantProfiles
            .Include(ap => ap.User)
            .FirstAsync(ap => ap.Id == id);

        return MapToResponse(profile);
    }

    private ApplicantProfileResponse MapToResponse(ApplicantProfile profile)
    {
        return new ApplicantProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            UserEmail = profile.User.Email!,
            UserName = $"{profile.User.FirstName} {profile.User.LastName}",
            MonthlyIncome = profile.MonthlyIncome,
            EmployerName = profile.EmployerName,
            GovernmentIdType = profile.GovernmentIdType,
            GovernmentIdNumber = profile.GovernmentIdNumber,
            NumberOfOccupants = profile.NumberOfOccupants,
            HasPets = profile.HasPets,
            PetDetails = profile.PetDetails,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }
}
