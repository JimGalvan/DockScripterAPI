using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Entities;

[Table("ScriptEntities")]
public class ScriptEntity : BaseEntity
{
    [Required] [MaxLength(100)] public string Name { get; set; }

    [Required] public string FilePath { get; set; } // Location of the script file

    [Required] public ScriptLanguage Language { get; set; } // Enum for language type, currently only Python

    [MaxLength(500)] public string? Description { get; set; }

    // Relationships
    [Required] public Guid UserId { get; set; } // Foreign key for the User who owns this script
    public UserEntity User { get; set; }

    // For tracking execution status
    public ScriptStatus Status { get; set; } = ScriptStatus.Created;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastExecutedAt { get; set; }
}