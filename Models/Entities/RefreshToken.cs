using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentEZApi.Models.Entities;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public DateTime ExpiresAt { get; set; }
    
    public DateTime? RevokedAt { get; set; }
    
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    
    [NotMapped]
    public bool IsRevoked => RevokedAt.HasValue;
    
    [NotMapped]
    public bool IsValid => !IsExpired && !IsRevoked;
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}