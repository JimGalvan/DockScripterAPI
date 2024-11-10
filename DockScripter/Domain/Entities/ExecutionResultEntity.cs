using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Entities;

[Table("ExecutionResultEntities")]
public class ExecutionResultEntity : BaseEntity
{
    [Required] public string Output { get; set; } // Stores the output of the script execution

    public string? ErrorOutput { get; set; } // Stores any error output if the script fails

    [Required] public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    [Required] public Guid ScriptId { get; set; } // Foreign key linking to the executed script
    public ScriptEntity Script { get; set; }

    public ExecutionStatus Status { get; set; } = ExecutionStatus.Pending;
}