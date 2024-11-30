using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DockScripter.Domain.Dtos.Requests;

public class CreateScriptRequestDto
{
    [Required] public string? Name { get; set; }
    [Required] public string? Description { get; set; }
    [Required] public string? EntryFilePath { get; set; }
    [Required] public string? Language { get; set; }
    [Required] public string? DockerImage { get; set; }
    public List<IFormFile>? Files { get; set; }
}