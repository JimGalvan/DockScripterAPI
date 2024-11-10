using System.ComponentModel.DataAnnotations;
using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Entities;

public class EnvironmentEntity : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string EnvironmentName { get; set; } = "Python"; // Default to Python for now

    [Required]
    public EnvironmentStatus Status { get; set; } = EnvironmentStatus.Initializing;

    public string? ContainerId { get; set; } // Docker container ID

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    [Required]
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
} 
