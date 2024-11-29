using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Dtos.Responses;

public class ExecutionResultResponseDto
{
    public Guid Id { get; set; }
    public string? Output { get; set; }
    public string? ErrorOutput { get; set; }
    public string? OutputFilePath { get; set; }
    public string? ErrorOutputFilePath { get; set; }
    public DateTime ExecutedAt { get; set; }
    public string? Status { get; set; }
}