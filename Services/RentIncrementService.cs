using Microsoft.EntityFrameworkCore;
using RentEZApi.Data;

public class RentIncrementService
{
    private readonly PropertyDbContext _dbContext;

    public RentIncrementService(PropertyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task IncrementMonthlyRent()
    {
        var activeApplications = await _dbContext.PropertyApplications
            .Include(pa => pa.Property)
            .Where(pa => pa.IsRenting)
            .ToListAsync();

        foreach (var app in activeApplications)
        {
            app.RentAmount += app.Property.Rent;
            app.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }
}
