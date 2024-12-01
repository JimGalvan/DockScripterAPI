using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Entities;

namespace DockScripter.Services.Interfaces;

public interface IScriptService
{
    Task<ScriptEntity> CreateScriptAsync(ScriptRequestDto scriptDto, DockerContainerEntity dockerContainer,
        HttpContext httpContext,
        CancellationToken cancellationToken);

    Task<IEnumerable<ScriptEntity>> GetAllScriptsAsync(Guid userId, CancellationToken cancellationToken);

    Task<ScriptEntity?> GetScriptByIdAsync(Guid scriptId, CancellationToken cancellationToken);

    Task<ScriptEntity?> UpdateScriptAsync(Guid scriptId, ScriptRequestDto scriptDto,
        CancellationToken cancellationToken);

    Task DeleteScriptAsync(Guid scriptId, CancellationToken cancellationToken);

    Task<string> AddScriptFileAsync(Guid scriptId, IFormFile file, CancellationToken cancellationToken);
}