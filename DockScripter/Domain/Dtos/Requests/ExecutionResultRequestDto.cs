namespace DockScripter.Domain.Dtos.Requests;

public class ExecutionResultRequestDto
{
    public string Output { get; set; }
    public string ErrorOutput { get; set; }
    public string Status { get; set; }
}