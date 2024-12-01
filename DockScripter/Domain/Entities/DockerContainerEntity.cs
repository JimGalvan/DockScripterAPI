using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Entities;

[Table("DockerContainerEntities")]
public class DockerContainerEntity : BaseEntity
{
    [MaxLength(255)] public string? DockerContainerName { get; set; }
    [Required] public DockerContainerStatus Status { get; set; }
    [Required] [MaxLength(255)] public string? DockerImage { get; set; }
    [MaxLength(64)] public string? ContainerId { get; set; }

    // Relationships
    [Required] public Guid UserId { get; init; }
    public UserEntity? User { get; set; }
}