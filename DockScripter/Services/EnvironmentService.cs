using DockScripter.Domain.Entities;
using DockScripter.Repositories;

namespace DockScripter.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly EnvironmentRepository _environmentRepository;

    public EnvironmentService(EnvironmentRepository environmentRepository)
    {
        _environmentRepository = environmentRepository;
    }

    public async Task<EnvironmentEntity?> GetEnvironmentByIdAsync(Guid environmentId,
        CancellationToken cancellationToken) =>
        await _environmentRepository.SelectById(environmentId, cancellationToken);

    public async Task<bool> InitializeEnvironmentAsync(EnvironmentEntity environment,
        CancellationToken cancellationToken)
    {
        await _environmentRepository.AddAsync(environment, cancellationToken);
        await _environmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> TerminateEnvironmentAsync(Guid environmentId, CancellationToken cancellationToken) =>
        await _environmentRepository.DeleteById(environmentId, cancellationToken);
}