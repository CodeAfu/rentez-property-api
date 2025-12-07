using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentEZApi.Models;
using RentEZApi.Models.Entities;

namespace RentEZApi.Data;

public class PropertyDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public PropertyDbContext(DbContextOptions<PropertyDbContext> options)
        : base(options)
    {
    }

    public DbSet<DocuSealPDFTemplate> DocuSealPDFTemplates { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Property> PropertyListings { get; set; } = null!;
    public DbSet<PropertyApplication> PropertyApplications { get; set; } = null!;
    public DbSet<DocuSealLeaseSubmission> DocuSealLeaseSubmissions { get; set; } = null!;

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.ConfigureNpgsql();
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTimestamps(modelBuilder);
        modelBuilder.Entity<User>().ToTable("Users");

        modelBuilder.Entity<User>(entity =>
        {
            // From IdentityUser
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

            // Constraint
            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_User_Age", "\"Age\" IS NULL OR (\"Age\" >= 18 AND \"Age\" <= 120)"
                ));

            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.PasswordHash)
                .HasColumnType("text")
                .IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => new { e.OwnerId, e.Hash }).IsUnique();
        });

        modelBuilder.Entity<DocuSealPDFTemplate>(entity =>
        {
            entity.HasOne(d => d.Owner)
                .WithMany(u => u.Templates)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.TemplateId).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.TokenHash);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpiresAt);
        });

        modelBuilder.Entity<PropertyApplication>(entity =>
        {
            entity.HasIndex(e => e.PropertyId);
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<DocuSealLeaseSubmission>(entity =>
        {
            entity.HasIndex(e => e.SubmissionId);
            entity.HasIndex(e => e.PropertyId);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.ExternalId);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void ConfigureTimestamps(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITimestampedEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType, builder =>
                {
                    builder.Property(nameof(ITimestampedEntity.CreatedAt))
                        .HasDefaultValueSql("NOW()")
                        .ValueGeneratedOnAdd();
                });
            }
        }
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<ITimestampedEntity>()
            .Where(e => e.State == EntityState.Modified);
        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
