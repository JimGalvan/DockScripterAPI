using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Entities;

[Table("ExecutionResultEntities")]
public class ExecutionResultEntity : BaseEntity
{
    [Required] [MaxLength(1024)] public string? Output { get; set; }

    [MaxLength(512)] public string? ErrorOutput { get; set; }

    [Required] public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    [Required] public Guid ScriptId { get; init; }
    public ScriptEntity? Script { get; set; }

    public ExecutionStatus Status { get; set; } = ExecutionStatus.Pending;
}