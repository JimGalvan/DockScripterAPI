using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Dtos.Responses;
using DockScripter.Domain.Entities;
using DockScripter.Domain.Enums;
using DockScripter.Services;
using Microsoft.AspNetCore.Mvc;

namespace DockScripter.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var script = new ScriptEntity
        {
            Name = scriptDto.Name,
            Description = scriptDto.Description,
            Language = ScriptLanguage.Python
        };

        await _scriptService.CreateScriptAsync(script, cancellationToken);
        
        return CreatedAtAction(nameof(GetScript), new { id = script.Id }, new ScriptResponseDto
        {
            Id = script.Id,
            Name = script.Name,
            Description = script.Description,
            Language = script.Language.ToString(),
            Status = script.Status.ToString(),
            CreatedAt = script.CreatedAt
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
            CreatedAt = script.CreatedAt,
            LastExecutedAt = script.LastExecutedAt
        };
    }
}