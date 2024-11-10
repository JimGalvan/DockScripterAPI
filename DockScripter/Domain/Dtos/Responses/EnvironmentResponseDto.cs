namespace DockScripter.Domain.Dtos.Responses;

public class EnvironmentResponseDto
{
    public Guid Id { get; set; }
    public string EnvironmentName { get; set; }
    public string Status { get; set; }
    public DateTime CreationDateTimeUtc { get; set; }
}