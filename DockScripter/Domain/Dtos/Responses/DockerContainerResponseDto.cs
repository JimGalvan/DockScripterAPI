namespace DockScripter.Domain.Dtos.Responses;

public class DockerContainerResponseDto
{
    public Guid Id { get; set; }
    public string DockerContainer { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}