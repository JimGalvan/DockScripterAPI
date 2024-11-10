using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Entities;

[Table("EnvironmentEntities")]
public class EnvironmentEntity : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string EnvironmentName { get; set; } = "Python"; // Default to Python for now

    [Required]
    public EnvironmentStatus Status { get; set; } = EnvironmentStatus.Initializing;

    public string? ContainerId { get; set; } // Docker container ID
    
    // Relationships
    [Required]
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
} 
