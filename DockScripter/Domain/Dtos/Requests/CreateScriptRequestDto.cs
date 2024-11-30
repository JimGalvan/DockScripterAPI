using System.ComponentModel.DataAnnotations;

namespace DockScripter.Domain.Dtos.Requests;

public class CreateScriptRequestDto
{
    [Required] public string? Name { get; set; }
    [Required] public string? Description { get; set; }
    [Required] public string? EntryFilePath { get; set; }
    [Required] public string? Language { get; set; }
}