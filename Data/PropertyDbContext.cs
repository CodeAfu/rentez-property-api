using Microsoft.EntityFrameworkCore;
using RentEZApi.Models;
using RentEZApi.Models.Entities;

namespace RentEZApi.Data;

public class PropertyDbContext : DbContext
{
    public PropertyDbContext(DbContextOptions<PropertyDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();
            
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.EmailAddress).IsUnique();
            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_User_Age", "age >= 18 AND age <= 120"
                ));
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("NOW()")
                .ValueGeneratedOnAdd();
        });
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<ITimestampedEntity>()
            .Where(e => e.State == EntityState.Modified);
        
        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries<ITimestampedEntity>()
            .Where(e => e.State == EntityState.Modified);
        
        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        
        return base.SaveChanges();
    }
}