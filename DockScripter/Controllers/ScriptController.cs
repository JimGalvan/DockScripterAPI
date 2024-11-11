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

    public ScriptController(IExecutionService executionService, IScriptService scriptService, IS3Service s3Service)
    {
        _executionService = executionService;
        _scriptService = scriptService;
        _s3Service = s3Service;
    }

    [HttpPost("{scriptId}/upload")]
    public async Task<IActionResult> UploadFile(Guid scriptId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest($"Invalid File. File length is {file.Length}.");

        // Generate a unique file key for S3
        var s3Key = $"{scriptId}/{file.FileName}";

        // Upload the file to S3
        using (var stream = file.OpenReadStream())
        {
            await _s3Service.UploadFileAsync(stream, s3Key);
        }

        // Add file metadata to the script in the database
        await _scriptService.AddScriptFileAsync(scriptId, s3Key, cancellationToken);

        return Ok(new { Message = "File uploaded successfully.", FileKey = s3Key });
    }

    [HttpPost("execute/{scriptId}")]
    public async Task<IActionResult> ExecuteScriptInContainerAsync(Guid scriptId, CancellationToken cancellationToken)
    {
        // Retrieve the script to ensure it exists and is accessible to the user
        var script = await _scriptService.GetScriptByIdAsync(scriptId, cancellationToken);
        if (script == null)
            return NotFound(new { Message = "Script not found." });

        // Execute the script in a Docker container
        var executionResult = await _executionService.ExecuteScriptInContainerAsync(script, cancellationToken);

        // Return the execution result
        return Ok(new ExecutionResultResponseDto
        {
            Id = executionResult.Id,
            Output = executionResult.Output!,
            ErrorOutput = executionResult.ErrorOutput!,
            Status = executionResult.Status.ToString(),
            ExecutedAt = executionResult.ExecutedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateScript(ScriptRequestDto scriptDto, CancellationToken cancellationToken)
    {
        var createdScript = await _scriptService.CreateScriptAsync(scriptDto, HttpContext, cancellationToken);

        return CreatedAtAction(nameof(GetScript), new { id = createdScript.Id }, new ScriptResponseDto
        {
            Id = createdScript.Id,
            Name = createdScript.Name,
            Description = createdScript.Description!,
            Language = createdScript.Language.ToString(),
            Status = createdScript.Status.ToString(),
            CreationDateTimeUtc = createdScript.CreationDateTimeUtc
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScriptResponseDto>> GetScript(Guid id, CancellationToken cancellationToken)
    {
        var script = await _scriptService.GetScriptByIdAsync(id, cancellationToken);
        if (script == null)
            return NotFound();

        return new ScriptResponseDto
        {
            Id = script.Id,
            Name = script.Name,
            Description = script.Description,
            Language = script.Language.ToString(),
            Status = script.Status.ToString(),
            CreationDateTimeUtc = script.CreationDateTimeUtc,
            LastExecutedAt = script.LastExecutedAt
        };
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