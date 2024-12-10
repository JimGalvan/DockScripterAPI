using System.Net;
using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Dtos.Responses;
using DockScripter.Services;
using DockScripter.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DockScripter.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ScriptController : ControllerBase
{
    private readonly IExecutionService _executionService;
    private readonly IScriptService _scriptService;
    private readonly IS3Service _s3Service;
    private readonly IDockerContainerService _dockerContainerService;
    private readonly IScriptTriggerService _scriptTriggerService;

    public ScriptController(IExecutionService executionService, IScriptService scriptService, IS3Service s3Service,
        IDockerContainerService dockerContainerService, IScriptTriggerService scriptTriggerService)
    {
        _executionService = executionService;
        _scriptService = scriptService;
        _s3Service = s3Service;
        _dockerContainerService = dockerContainerService;
        _scriptTriggerService = scriptTriggerService;
    }

    [HttpPost("{scriptId}/upload")]
    public async Task<IActionResult> UploadFile(Guid scriptId, IFormFile file, CancellationToken cancellationToken)
    {
        var s3Key = await _scriptService.AddScriptFileAsync(scriptId, file, cancellationToken);
        return Ok(new { Message = "File uploaded successfully.", FileKey = s3Key });
    }

    [HttpPost("{scriptId}/execute")]
    public async Task<IActionResult> ExecuteScriptInContainerAsync(Guid scriptId, CancellationToken cancellationToken)
    {
        var script = await _scriptService.GetScriptByIdAsync(scriptId, cancellationToken);
        if (script == null)
            return NotFound(new { Message = "Script not found." });

        var executionResult = await _executionService.ExecuteScriptInContainerAsync(script, cancellationToken);

        if (executionResult.Status == Domain.Enums.ExecutionStatus.Failed)
            return BadRequest(new { Message = executionResult.ErrorOutput });

        return Ok(new ExecutionResultResponseDto
        {
            Id = executionResult.Id,
            Output = executionResult.Output!,
            Status = executionResult.Status.ToString(),
            ExecutedAt = executionResult.ExecutedAt,
            OutputFilePath = executionResult.OutputFilePath,
            ErrorOutput = executionResult.ErrorOutput,
            ErrorOutputFilePath = executionResult.ErrorOutputFilePath
        });
    }

    [HttpPost("{scriptId}/start")]
    public async Task<IActionResult> TriggerScriptExecutionAsync(Guid scriptId, CancellationToken cancellationToken)
    {
        var script = await _scriptService.GetScriptByIdAsync(scriptId, cancellationToken);
        if (script == null)
            return NotFound(new { Message = "Script not found." });

        if (script.Name == null)
        {
            return BadRequest(new { Message = "Script name is not set." });
        }

        var parameters = new Dictionary<string, string>
            { { "eventType", "script.execute" }, { "source", "DockScripter" } };
        var response = await _scriptTriggerService.SendScriptTriggerAsync(scriptId, parameters);

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            return BadRequest(new { Message = "Failed to trigger script execution." });
        }

        return Ok(new { Message = "Script execution triggered successfully." });
    }


    [HttpPost]
    public async Task<IActionResult> CreateScript(ScriptRequestDto scriptDto, CancellationToken cancellationToken)
    {
        var createDockerContainerDto = new DockerContainerRequestDto
        {
            DockerImage = scriptDto.DockerImage
        };

        var dockerContainer =
            await _dockerContainerService.StoreDockerContainerDataAsync(createDockerContainerDto, HttpContext,
                cancellationToken);

        var createdScript =
            await _scriptService.CreateScriptAsync(scriptDto, dockerContainer, HttpContext,
                cancellationToken);

        var responseDto = new ScriptResponseDto
        {
            Id = createdScript.Id,
            Name = createdScript.Name!,
            EntryFilePath = createdScript.EntryFilePath!,
            Description = createdScript.Description!,
            Language = createdScript.Language.ToString(),
            Status = createdScript.Status.ToString(),
            CreationDateTimeUtc = createdScript.CreationDateTimeUtc,
            Files = createdScript.Files
        };
        return CreatedAtAction(nameof(CreateScript), new { id = createdScript.Id }, responseDto);
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ScriptResponseDto>>> GetAllScripts(CancellationToken cancellationToken)
    {
        var userId = ControllerUtils.GetUserIdFromToken(this);
        var scripts = await _scriptService.GetAllScriptsAsync(userId, cancellationToken);
        var response = scripts.Select(script => new ScriptResponseDto
        {
            Id = script.Id,
            Name = script.Name,
            Description = script.Description,
            Language = script.Language.ToString(),
            Status = script.Status.ToString(),
            CreationDateTimeUtc = script.CreationDateTimeUtc,
            LastExecutedAt = script.LastExecutedAt,
            Files = script.Files
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScriptResponseDto>> GetScript(Guid id, CancellationToken cancellationToken)
    {
        var script = await _scriptService.GetScriptByIdAsync(id, cancellationToken);
        if (script == null)
            return NotFound();

        var response = new ScriptResponseDto
        {
            Id = script.Id,
            Name = script.Name,
            Description = script.Description,
            Language = script.Language.ToString(),
            Status = script.Status.ToString(),
            CreationDateTimeUtc = script.CreationDateTimeUtc,
            LastExecutedAt = script.LastExecutedAt,
            Files = script.Files
        };
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateScript(Guid id, ScriptRequestDto scriptDto,
        CancellationToken cancellationToken)
    {
        var updatedScript = await _scriptService.UpdateScriptAsync(id, scriptDto, cancellationToken);
        if (updatedScript == null)
            return NotFound();

        return Ok(new ScriptResponseDto
        {
            Id = updatedScript.Id,
            Name = updatedScript.Name!,
            Description = updatedScript.Description!,
            Language = updatedScript.Language.ToString(),
            Status = updatedScript.Status.ToString(),
            CreationDateTimeUtc = updatedScript.CreationDateTimeUtc
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScript(Guid id, CancellationToken cancellationToken)
    {
        await _scriptService.DeleteScriptAsync(id, cancellationToken);
        return NoContent();
    }
}