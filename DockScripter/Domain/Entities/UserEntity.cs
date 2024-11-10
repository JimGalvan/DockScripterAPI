using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DockScripter.Domain.Entities;

public class UserEntity : BaseEntity
{
    [Required] [MaxLength(50)] public string FirstName { get; set; }

    [Required] [MaxLength(50)] public string LastName { get; set; }

    public byte[]? PasswordHash { get; init; }
    public byte[]? PasswordSalt { get; init; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; init; }

    // Relationships
    public ICollection<ScriptEntity> Scripts { get; set; } = new List<ScriptEntity>();
    public ICollection<EnvironmentEntity> Environments { get; set; } = new List<EnvironmentEntity>();
}