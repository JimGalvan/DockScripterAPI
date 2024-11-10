using DockScripter.Domain.Entities;

namespace DockScripter.Services;

public interface IScriptService
{
    Task<ScriptEntity?> GetScriptByIdAsync(Guid scriptId, CancellationToken cancellationToken);
    Task<bool> CreateScriptAsync(ScriptEntity script, CancellationToken cancellationToken);
    Task<bool> UpdateScriptAsync(ScriptEntity script, CancellationToken cancellationToken);
    Task<bool> DeleteScriptAsync(Guid scriptId, CancellationToken cancellationToken);
}