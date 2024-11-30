using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Entities;

[Table("EnvironmentEntities")]
public class EnvironmentEntity : BaseEntity
{
    [Required] [MaxLength(50)] public string EnvironmentName { get; set; } = "Untitled";
    [Required] public EnvironmentStatus Status { get; set; } = EnvironmentStatus.NotInitialized;
    [Required] [MaxLength(255)] public string? DockerImage { get; set; }
    [MaxLength(64)] public string? ContainerId { get; set; }

    // Relationships
    [Required] public Guid UserId { get; init; }
    public UserEntity? User { get; set; }
}