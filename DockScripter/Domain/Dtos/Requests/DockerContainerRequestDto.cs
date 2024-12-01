using System.ComponentModel.DataAnnotations;

namespace DockScripter.Domain.Dtos.Requests;

public class DockerContainerRequestDto
{
    [Required] public string? DockerImage { get; set; }
    public string? DockerContainerName { get; set; } = null;
}