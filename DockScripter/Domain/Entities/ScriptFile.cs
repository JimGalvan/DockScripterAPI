using System.ComponentModel.DataAnnotations;

namespace DockScripter.Domain.Entities;

public class ScriptFile : BaseEntity
{
    [Required] [MaxLength(1024)] public string S3Key { get; set; }
    [Required] public Guid ScriptId { get; set; }

    // Relationships
    public ScriptEntity Script { get; set; }
}