using System.ComponentModel.DataAnnotations;
using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Dtos.Requests;

public class ScriptRequestDto
{
    [Required] public string Name { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string EntryFilePath { get; set; }
    [Required] public string Language { get; set; }
}