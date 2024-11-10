using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Entities;

namespace DockScripter.Services.Interfaces;

public interface IScriptService
{
    Task<ScriptEntity> CreateScriptAsync(ScriptRequestDto scriptDto, HttpContext httpContext,
        CancellationToken cancellationToken);

    Task<ScriptEntity?> GetScriptByIdAsync(Guid scriptId, CancellationToken cancellationToken);
}