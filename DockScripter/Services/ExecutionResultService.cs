using DockScripter.Domain.Entities;
using DockScripter.Repositories;

namespace DockScripter.Services;

public class ExecutionResultService : IExecutionResultService
{
    private readonly ExecutionResultRepository _executionResultRepository;

    public ExecutionResultService(ExecutionResultRepository executionResultRepository)
    {
        _executionResultRepository = executionResultRepository;
    }

    public async Task<ExecutionResultEntity?> GetResultByIdAsync(Guid resultId, CancellationToken cancellationToken) =>
        await _executionResultRepository.SelectById(resultId, cancellationToken);

    public async Task<bool> LogExecutionResultAsync(ExecutionResultEntity result, CancellationToken cancellationToken)
    {
        await _executionResultRepository.AddAsync(result, cancellationToken);
        await _executionResultRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}