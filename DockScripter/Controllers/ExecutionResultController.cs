using DockScripter.Domain.Dtos.Responses;
using DockScripter.Services;
using Microsoft.AspNetCore.Mvc;

namespace DockScripter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExecutionResultController : ControllerBase
{
    private readonly IExecutionResultService _executionResultService;

    public ExecutionResultController(IExecutionResultService executionResultService)
    {
        _executionResultService = executionResultService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExecutionResultResponseDto>> GetExecutionResult(Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _executionResultService.GetResultByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound();

        return new ExecutionResultResponseDto
        {
            Id = result.Id,
            Output = result.Output,
            ErrorOutput = result.ErrorOutput,
            Status = result.Status.ToString(),
            ExecutedAt = result.ExecutedAt
        };
    }
}