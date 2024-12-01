using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DockScripter.Domain.Entities;

public class UserEntity : BaseEntity
{
    [MaxLength(50)] public string? FirstName { get; init; }

    [MaxLength(50)] public string? LastName { get; init; }

    public byte[]? PasswordHash { get; init; }
    public byte[]? PasswordSalt { get; init; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string? Email { get; init; }

    // Relationships
    public ICollection<ScriptEntity>? Scripts { get; init; } = new List<ScriptEntity>();
    public ICollection<DockerContainerEntity>? DockerContainers { get; init; } = new List<DockerContainerEntity>();
}