using DockScripter.Domain.Entities;
using DockScripter.Repositories;

namespace DockScripter.Services;

public class ScriptService : IScriptService
{
    private readonly ScriptRepository _scriptRepository;

    public ScriptService(ScriptRepository scriptRepository)
    {
        _scriptRepository = scriptRepository;
    }

    public async Task<ScriptEntity?> GetScriptByIdAsync(Guid scriptId, CancellationToken cancellationToken) =>
        await _scriptRepository.SelectById(scriptId, cancellationToken);

    public async Task<bool> CreateScriptAsync(ScriptEntity script, CancellationToken cancellationToken)
    {
        await _scriptRepository.AddAsync(script, cancellationToken);
        await _scriptRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateScriptAsync(ScriptEntity script, CancellationToken cancellationToken)
    {
        _scriptRepository.objects?.Update(script);
        await _scriptRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteScriptAsync(Guid scriptId, CancellationToken cancellationToken) =>
        await _scriptRepository.DeleteById(scriptId, cancellationToken);
}