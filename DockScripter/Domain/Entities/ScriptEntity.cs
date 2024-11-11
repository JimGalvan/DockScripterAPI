using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Entities;

[Table("ScriptEntities")]
public class ScriptEntity : BaseEntity
{
    [Required] [MaxLength(100)] public string? Name { get; set; }
    [Required] [MaxLength(260)] public string? EntryFilePath { get; set; }
    [MaxLength(260)] public string? FilePath { get; set; }
    [Required] public ScriptLanguage Language { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    public ScriptStatus Status { get; init; } = ScriptStatus.Created;
    public DateTime? LastExecutedAt { get; init; }

    // Relationships
    [Required] public Guid UserId { get; init; }
    public UserEntity? User { get; init; }
    public ICollection<ScriptFile> Files { get; set; } = new List<ScriptFile>();
}