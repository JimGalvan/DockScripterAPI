using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Entities;

namespace DockScripter.Domain.Dtos.Responses;

public class ScriptResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }
    public string Status { get; set; }

    public string EntryFilePath { get; set; }

    public ICollection<ScriptFile> Files { get; set; } = new List<ScriptFile>();
    public DateTime CreationDateTimeUtc { get; set; }
    public DateTime? LastExecutedAt { get; set; }
}