using DockScripter.Domain.Entities;

namespace DockScripter.Services;

public interface IEnvironmentService
{
    Task<EnvironmentEntity?> GetEnvironmentByIdAsync(Guid environmentId, CancellationToken cancellationToken);
    Task<bool> InitializeEnvironmentAsync(EnvironmentEntity environment, CancellationToken cancellationToken);
    Task<bool> TerminateEnvironmentAsync(Guid environmentId, CancellationToken cancellationToken);
}