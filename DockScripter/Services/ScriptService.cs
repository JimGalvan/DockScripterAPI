using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Entities;
using DockScripter.Domain.Enums;
using DockScripter.Repositories;
using System.Security.Claims;
using DockScripter.Core.Exceptions;
using DockScripter.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DockScripter.Services;

public class ScriptService : IScriptService
{
    private readonly ScriptRepository _scriptRepository;
    private readonly ScriptFileRepository _scriptFileRepository;
    private readonly IS3Service _s3Service;

    public ScriptService(ScriptRepository scriptRepository, ScriptFileRepository scriptFileRepository,
        IS3Service s3Service)
    {
        _scriptRepository = scriptRepository;
        _scriptFileRepository = scriptFileRepository;
        _s3Service = s3Service;
    }

    public async Task<string> AddScriptFileAsync(Guid scriptId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            throw new ArgumentException($"Invalid File. File length is {file.Length}.");

        var script = await _scriptRepository.SelectById(scriptId, cancellationToken);

        if (script == null)
            throw new ArgumentException("Script not found.");

        var s3Key = $"{scriptId}/{file.FileName}";

        var wasFileUploaded = false;

        await using (var stream = file.OpenReadStream())
        {
            var response = await _s3Service.UploadFileAsync(stream, s3Key);
            wasFileUploaded = response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        if (!wasFileUploaded)
            throw new S3FileUploadException("Failed to upload file to S3.");

        var scriptFile = new ScriptFile
        {
            ScriptId = scriptId,
            S3Key = s3Key
        };

        await _scriptFileRepository.AddAsync(scriptFile, cancellationToken);
        await _scriptFileRepository.SaveChangesAsync(cancellationToken);

        script.Files.Add(scriptFile);
        await _scriptRepository.SaveChangesAsync(cancellationToken);

        return s3Key;
    }

    public async Task<ScriptEntity> CreateScriptAsync(ScriptRequestDto scriptDto, DockerContainerEntity dockerContainer,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                throw new UnauthorizedAccessException("User not authenticated"));

        if (!Enum.TryParse(scriptDto.Language, out ScriptLanguage language))
        {
            throw new ArgumentException("Invalid language specified.");
        }

        var script = new ScriptEntity
        {
            Name = scriptDto.Name,
            Description = scriptDto.Description,
            EntryFilePath = scriptDto.EntryFilePath,
            Language = language,
            UserId = userId,
            DockerContainer = dockerContainer,
        };

        await _scriptRepository.AddAsync(script, cancellationToken);
        await _scriptRepository.SaveChangesAsync(cancellationToken);

        var createdScript = _scriptRepository.SelectById(script.Id, cancellationToken).Result;
        var scriptId = createdScript?.Id ?? throw new Exception("Script ID is null.");

        if (scriptDto.Files == null || scriptDto.Files.Count == 0)
        {
            return createdScript;
        }

        foreach (var file in scriptDto.Files)
        {
            try
            {
                await AddScriptFileAsync(scriptId, file, cancellationToken);
            }
            catch (S3FileUploadException e)
            {
                await _scriptRepository.DeleteById(scriptId, cancellationToken);
                await _scriptRepository.SaveChangesAsync(cancellationToken);
                throw new Exception("Failed to upload file to S3. Script creation failed.", e);
            }
        }

        var createdScriptWithFiles = _scriptRepository.SelectById(scriptId, cancellationToken).Result;
        if (createdScriptWithFiles == null)
        {
            throw new Exception("Script is null.");
        }

        return createdScriptWithFiles;
    }

    public async Task<ScriptEntity?> GetScriptByIdAsync(Guid scriptId, CancellationToken cancellationToken)
    {
        return await _scriptRepository.SelectById(scriptId, cancellationToken, s => s.Files);
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