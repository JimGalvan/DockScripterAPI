using DockScripter.Domain.Dtos.Responses;
using DockScripter.Domain.Entities;
using DockScripter.Services;
using DockScripter.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DockScripter.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ExecutionResultController : ControllerBase
{
    private readonly IExecutionService _executionService;

    public ExecutionResultController(IExecutionService executionService)
    {
        _executionService = executionService;
    }

    [HttpGet("{scriptId}")]
    public async Task<ActionResult<IEnumerable<ExecutionResultEntity>>> GetExecutionResults(Guid scriptId, CancellationToken cancellationToken)
    {
        var results = await _executionService.GetResultsByScriptId(scriptId, cancellationToken);
        return Ok(results);
    }
}
