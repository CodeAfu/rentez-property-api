using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;
using RentEZApi.Models.Entities;
using RentEZApi.Exceptions;
using RentEZApi.Models.DTOs.Property;

namespace RentEZApi.Services;

public class PropertyService
{
    private readonly PropertyDbContext _dbContext;

    public PropertyService(PropertyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Property>> GetPaginatedAsync(int pageNum, int lim, string search)
    {
        var query = _dbContext.Property.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(search) ||
                p.Description.ToLower().Contains(search) ||
                p.Address.ToLower().Contains(search) ||
                p.City.ToLower().Contains(search) ||
                p.State.ToLower().Contains(search)
            );
        }

        return await _dbContext.Property
            .OrderBy(p => p.Id)
            .Skip((pageNum - 1) * lim)
            .Take(lim)
            .ToListAsync();

    }

    public async Task<Property?> Get(Guid id)
        => await _dbContext.Property.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Property> CreateAsync(CreatePropertyDto dto, Guid ownerId)
    {
        var hash = Hasher.Hash(new { dto, ownerId });
        var matchingHash = await _dbContext.Property
            .FirstOrDefaultAsync(p => p.OwnerId == ownerId && p.Hash == hash);

        if (matchingHash != null)
        {
            throw new DuplicateObjectException("Attempted to create a listing with the same hash value");
        }

        var property = new Property
        {
            OwnerId = ownerId,
            Title = dto.Title,
            Hash = hash,
            Description = dto.Description,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Images = dto.Images,
            DepositRequired = dto.DepositRequired,
            RoomType = dto.RoomType,
            PreferredRaces = dto.PreferredRaces,
            PreferredOccupation = dto.PreferredOccupation,
            LeaseTermCategory = dto.LeaseTermCategory,
            BillsIncluded = dto.BillsIncluded,
        };

        var result = _dbContext.Property.Add(property);
        await _dbContext.SaveChangesAsync();

        return property;
    }

    public async Task<Property> Edit(Guid id, EditPropertyDto request)
    {
        var property = await _dbContext.Property.FirstOrDefaultAsync(p => p.Id == id);
        if (property == null)
            throw new ObjectNotFoundException(id);

        if (!string.IsNullOrWhiteSpace(request.Title))
            property.Title = request.Title;

        if (!string.IsNullOrWhiteSpace(request.Description))
            property.Description = request.Description;

        if (!string.IsNullOrWhiteSpace(request.Address))
            property.Address = request.Address;

        if (!string.IsNullOrWhiteSpace(request.City))
            property.City = request.City;

        if (request.Rent != null)
            property.Rent = request.Rent.Value;

        if (request.Images != null && request.Images.Length > 0)
            property.Images = request.Images;

        if (request.DepositRequired != null)
            property.DepositRequired = request.DepositRequired;

        if (request.BillsIncluded != null)
            property.BillsIncluded = request.BillsIncluded;

        if (request.RoomType != null && request.RoomType.Length > 0)
            property.RoomType = request.RoomType;

        if (request.PreferredRaces != null && request.PreferredRaces.Length > 0)
            property.PreferredRaces = request.PreferredRaces;

        if (request.PreferredOccupation != null && request.PreferredOccupation.Length > 0)
            property.PreferredOccupation = request.PreferredOccupation;

        if (request.LeaseTermCategory != null && request.LeaseTermCategory.Length > 0)
            property.LeaseTermCategory = request.LeaseTermCategory;

        property.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return property;
    }
}
