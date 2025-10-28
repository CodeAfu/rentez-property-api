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
    public DbSet<DocuSealPDFTemplate> DocuSealPDFTemplates { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureNpgsql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTimestamps(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            // From IdentityUser
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email_address").HasMaxLength(256).IsRequired();
            entity.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasColumnName("normalized_email").HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasColumnName("normalized_user_name").HasMaxLength(256);
            entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
            entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
            entity.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
            entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
            entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            entity.Property(e => e.TwoFactorEnabled).HasColumnName("two_factor_enabled");

            // Constraint
            entity.HasIndex(e => e.Email).IsUnique();
            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_User_Age", "age >= 18 AND age <= 120"
                ));

            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(30);
            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .HasColumnType("text")
                .IsRequired();
        });

        modelBuilder.Entity<DocuSealPDFTemplate>(entity =>
        {
            entity.HasOne(d => d.Owner)
                .WithMany(u => u.Templates)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.TemplateId).IsUnique();
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
                    
                    builder.Property(nameof(ITimestampedEntity.UpdatedAt))
                        .HasDefaultValueSql("NOW()")
                        .ValueGeneratedOnAddOrUpdate();
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