using DockScripter.Domain.Entities;

namespace DockScripter.Services;

public interface IExecutionResultService
{
    Task<ExecutionResultEntity?> GetResultByIdAsync(Guid resultId, CancellationToken cancellationToken);
    Task<bool> LogExecutionResultAsync(ExecutionResultEntity result, CancellationToken cancellationToken);
}