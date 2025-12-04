using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;
using RentEZApi.Models.Entities;
using RentEZApi.Exceptions;
using RentEZApi.Models.DTOs.Property;
using RentEZApi.Models.DTOs.Result;

namespace RentEZApi.Services;

public class PropertyService
{
    private readonly PropertyDbContext _dbContext;
    private readonly ILogger<PropertyService> _logger;

    public PropertyService(PropertyDbContext dbContext, ILogger<PropertyService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PaginatedResult<PropertyListDto>> GetPaginatedAsync(PropertyFilterRequest filters)
    {
        var query = _dbContext.Property
            .Include(p => p.Owner)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(search) ||
                p.Description!.ToLower().Contains(search) ||
                p.Address.ToLower().Contains(search) ||
                p.City.ToLower().Contains(search) ||
                p.State.ToLower().Contains(search)
            );
        }

        if (filters.RoomTypes?.Length > 0)
        {
            query = query.Where(p => p.RoomType.Any(rt => filters.RoomTypes.Contains(rt)));
        }

        if (!string.IsNullOrWhiteSpace(filters.City))
        {
            query = query.Where(p => p.City.ToLower() == filters.City.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(filters.State))
        {
            query = query.Where(p => p.State.ToLower() == filters.State.ToLower());
        }

        if (filters.MinRent.HasValue)
        {
            query = query.Where(p => p.Rent >= filters.MinRent.Value);
        }

        if (filters.MaxRent.HasValue)
        {
            query = query.Where(p => p.Rent <= filters.MaxRent.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.OwnerName))
        {
            var ownerName = filters.OwnerName.ToLower();
            query = query.Where(p =>
                (p.Owner.FirstName + " " + p.Owner.LastName).ToLower().Contains(ownerName)
            );
        }
        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((filters.PageNum - 1) * filters.Lim)
            .Take(filters.Lim)
            .Select(p => new PropertyListDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description!,
                Address = p.Address,
                City = p.City,
                State = p.State,
                OwnerFirstName = p.Owner.FirstName,
                OwnerLastName = p.Owner.LastName,
                Rent = p.Rent,
                Images = p.Images,
                RoomType = p.RoomType
            })
            .ToListAsync();

        return new PaginatedResult<PropertyListDto>
        {
            Items = items,
            Pagination = new Pagination
            {
                PageNum = filters.PageNum,
                PageSize = filters.Lim,
                TotalCount = totalCount,
            }
        };
    }

    public async Task<Property?> Get(Guid id)
        => await _dbContext.Property.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<ICollection<PropertyDetails>> GetUserOwnedProperty(Guid userId)
    {
        return await _dbContext.Property
            .AsNoTracking()
            .Where(u => u.OwnerId == userId)
            .Select(p => new PropertyDetails
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description ?? "",
                Address = p.Address,
                City = p.City,
                State = p.State,
                Rent = p.Rent,
                Images = p.Images,
                DepositRequired = p.DepositRequired,
                BillsIncluded = p.BillsIncluded,
                RoomType = p.RoomType,
                PreferredRaces = p.PreferredRaces,
                PreferredOccupation = p.PreferredOccupation,
                LeaseTermCategory = p.LeaseTermCategory,
                OwnerId = p.OwnerId,
                AgreementId = p.AgreementId,
                CreatedAt = p.CreatedAt,
            })
            .ToListAsync();
    }

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
            Rent = dto.Rent,
            City = dto.City,
            State = dto.State,
            Images = dto.Images,
            DepositRequired = dto.DepositRequired,
            RoomType = dto.RoomType,
            PreferredRaces = dto.PreferredRaces,
            PreferredOccupation = dto.PreferredOccupation,
            LeaseTermCategory = dto.LeaseTermCategory,
            BillsIncluded = dto.BillsIncluded!,
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
