using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DockScripter.Domain.Entities;

public class ScriptFile : BaseEntity
{
    [Required] [MaxLength(1024)] public string S3Key { get; set; }
    [Required] public Guid ScriptId { get; set; }

    // Relationships
    [JsonIgnore] public ScriptEntity? Script { get; set; }
}