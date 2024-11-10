namespace DockScripter.Domain.Dtos.Responses;

public class ExecutionResultResponseDto
{
    public Guid Id { get; set; }
    public string Output { get; set; }
    public string ErrorOutput { get; set; }
    public string Status { get; set; }
    public DateTime ExecutedAt { get; set; }
}