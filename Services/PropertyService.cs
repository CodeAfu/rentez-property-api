using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;
using RentEZApi.Models.Entities;
using RentEZApi.Exceptions;

namespace RentEZApi.Services;

public class PropertyService
{
    private readonly PropertyDbContext _dbContext;

    public PropertyService(PropertyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Property>> GetPaginatedAsync(int pageNum, int lim)
    {
        var propertyResult = await _dbContext.Property
            .OrderBy(p => p.Id)
            .Skip((pageNum - 1) * lim)
            .Take(lim)
            .ToListAsync();

        return propertyResult;
    }

    public async Task<Property?> Get(Guid id)
        => await _dbContext.Property.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Property> CreateAsync(CreatePropertyDto dto)
    {
        var hash = Hasher.Hash(dto);
        var matchingHash = await _dbContext.Property
            .FirstOrDefaultAsync(p => p.Hash == hash);

        if (matchingHash != null)
        {
            throw new DuplicateObjectException("Attempted to create a listing with the same hash value");
        }

        var property = new Property
        {
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
}
