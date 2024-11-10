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
    private readonly IScriptService _scriptService;

    public ScriptController(IScriptService scriptService)
    {
        _scriptService = scriptService;
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
}