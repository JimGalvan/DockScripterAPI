using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Entities;
using DockScripter.Domain.Enums;
using DockScripter.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DockScripter.Services.Interfaces;

namespace DockScripter.Services;

public class ScriptService : IScriptService
{
    private readonly ScriptRepository _scriptRepository;

    public ScriptService(ScriptRepository scriptRepository)
    {
        _scriptRepository = scriptRepository;
    }

    public async Task<ScriptEntity> CreateScriptAsync(ScriptRequestDto scriptDto, HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        // Get user ID from the token within the HttpContext
        var userId = Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                throw new UnauthorizedAccessException("User not authenticated"));

        // Parse the language
        if (!Enum.TryParse(scriptDto.Language, out ScriptLanguage language))
        {
            throw new ArgumentException("Invalid language specified.");
        }

        // Create and populate the ScriptEntity
        var script = new ScriptEntity
        {
            Name = scriptDto.Name,
            Description = scriptDto.Description,
            FilePath = scriptDto.FilePath,
            Language = language,
            UserId = userId
        };

        await _scriptRepository.AddAsync(script, cancellationToken);
        await _scriptRepository.SaveChangesAsync(cancellationToken);

        return script;
    }

    public async Task<ScriptEntity?> GetScriptByIdAsync(Guid scriptId, CancellationToken cancellationToken)
    {
        return await _scriptRepository.SelectById(scriptId, cancellationToken);
    }
}