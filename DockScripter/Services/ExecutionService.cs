using DockScripter.Domain.Entities;
using System.Diagnostics;
using DockScripter.Domain.Enums;
using DockScripter.Repositories;
using DockScripter.Services.Interfaces;

namespace DockScripter.Services;

public class ExecutionService : IExecutionService
{
    private readonly ExecutionResultRepository _executionResultRepository;

    public ExecutionService(ExecutionResultRepository executionResultRepository)
    {
        _executionResultRepository = executionResultRepository;
    }

    public async Task<ExecutionResultEntity?> GetResultsByScriptId(Guid scriptId, CancellationToken cancellationToken)
    {
        return await _executionResultRepository.SelectById(scriptId, cancellationToken);
    }

    public async Task<ExecutionResultEntity> ExecuteScriptAsync(ScriptEntity script,
        CancellationToken cancellationToken)
    {
        var result = new ExecutionResultEntity
        {
            ScriptId = script.Id,
            Status = ExecutionStatus.Running,
            ExecutedAt = DateTime.UtcNow
        };

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python", // assuming Python script execution
                    Arguments = script.FilePath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            result.Output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            result.ErrorOutput = await process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            result.Status = process.ExitCode == 0 ? ExecutionStatus.Success : ExecutionStatus.Failed;
        }
        catch (Exception ex)
        {
            result.Status = ExecutionStatus.Failed;
            result.ErrorOutput = ex.Message;
        }

        await _executionResultRepository.AddAsync(result, cancellationToken);
        await _executionResultRepository.SaveChangesAsync(cancellationToken);

        return result;
    }
}