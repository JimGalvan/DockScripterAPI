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
    private readonly ScriptFileRepository _scriptFileRepository;

    public ScriptService(ScriptRepository scriptRepository, ScriptFileRepository scriptFileRepository)
    {
        _scriptRepository = scriptRepository;
        _scriptFileRepository = scriptFileRepository;
    }


    public async Task AddScriptFileAsync(Guid scriptId, string s3Key, CancellationToken cancellationToken)
    {
        // Check if the script exists
        var script = await _scriptRepository.SelectById(scriptId, cancellationToken);
        if (script == null)
        {
            throw new ArgumentException("Script not found.");
        }

        // Create a new ScriptFile entry
        var scriptFile = new ScriptFile
        {
            ScriptId = scriptId,
            S3Key = s3Key
        };

        // Add the ScriptFile to the database and save changes
        await _scriptFileRepository.AddAsync(scriptFile, cancellationToken);
        await _scriptFileRepository.SaveChangesAsync(cancellationToken);

        // Add the ScriptFile to the Script entity
        script.Files.Add(scriptFile);
        await _scriptRepository.SaveChangesAsync(cancellationToken);

        var result = await _scriptRepository.SelectById(scriptId, cancellationToken);
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
            EntryFilePath = scriptDto.EntryFilePath,
            Language = language,
            UserId = userId
        };

        await _scriptRepository.AddAsync(script, cancellationToken);
        await _scriptRepository.SaveChangesAsync(cancellationToken);

        return script;
    }

    public async Task<ScriptEntity?> GetScriptByIdAsync(Guid scriptId, CancellationToken cancellationToken)
    {
        var script = await _scriptRepository.SelectById(scriptId, cancellationToken);
        return script;
    }

    public async Task<ScriptEntity?> UpdateScriptAsync(Guid scriptId, ScriptRequestDto scriptDto,
        CancellationToken cancellationToken)
    {
        var script = await _scriptRepository.SelectById(scriptId, cancellationToken);
        if (script == null)
        {
            throw new Exception("Script with ID: '%s' not found.".Replace("%s", scriptId.ToString()));
        }

        // Parse the language
        if (!Enum.TryParse(scriptDto.Language, out ScriptLanguage language))
        {
            throw new ArgumentException("Invalid language specified.");
        }

        script.Name = scriptDto.Name;
        script.Description = scriptDto.Description;
        script.EntryFilePath = scriptDto.EntryFilePath;
        script.Language = language;

        await _scriptRepository.SaveChangesAsync(cancellationToken);

        return script;
    }

    public async Task DeleteScriptAsync(Guid scriptId, CancellationToken cancellationToken)
    {
        await _scriptRepository.DeleteById(scriptId, cancellationToken);
        await _scriptRepository.SaveChangesAsync(cancellationToken);
    }
}