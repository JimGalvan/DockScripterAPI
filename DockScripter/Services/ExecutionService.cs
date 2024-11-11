using DockScripter.Domain.Entities;
using DockScripter.Domain.Enums;
using DockScripter.Repositories;
using DockScripter.Services;
using DockScripter.Services.Interfaces;

public class ExecutionService : IExecutionService
{
    private readonly DockerService _dockerService;
    private readonly IS3Service _s3Service;
    private readonly ExecutionResultRepository _executionResultRepository;

    public ExecutionService(DockerService dockerService, IS3Service s3Service,
        ExecutionResultRepository executionResultRepository)
    {
        _dockerService = dockerService;
        _s3Service = s3Service;
        _executionResultRepository = executionResultRepository;
    }

    public async Task<ExecutionResultEntity> ExecuteScriptInContainerAsync(ScriptEntity script,
        CancellationToken cancellationToken)
    {
        var result = new ExecutionResultEntity
        {
            ScriptId = script.Id,
            Status = ExecutionStatus.Running,
            ExecutedAt = DateTime.UtcNow
        };

        // Create a temporary directory to store the script files
        var tempDirectory = Path.Combine(Path.GetTempPath(), script.Id.ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            // Step 1: Download all files associated with the script from S3
            foreach (var scriptFile in script.Files)
            {
                var filePath = Path.Combine(tempDirectory, Path.GetFileName(scriptFile.S3Key));
                using (var downloadStream = await _s3Service.DownloadFileAsync(scriptFile.S3Key))
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await downloadStream.CopyToAsync(fileStream);
                }
            }

            // Step 2: Execute the main script file in a Docker container
            var containerId =
                await _dockerService.ExecuteScriptWithFilesAsync(tempDirectory, script.EntryFilePath,
                    cancellationToken);

            // Step 3: Capture output and status
            result.Output = "Execution started in Docker container";
            result.Status = ExecutionStatus.Success;

            // Optional: Implement further result fetching from Docker logs or output streams
        }
        catch (Exception ex)
        {
            result.Status = ExecutionStatus.Failed;
            result.ErrorOutput = ex.Message;
        }
        finally
        {
            // Cleanup the temporary directory after execution
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);
        }

        // Save execution result to the database
        await _executionResultRepository.AddAsync(result, cancellationToken);
        await _executionResultRepository.SaveChangesAsync(cancellationToken);

        return result;
    }

    public async Task<ExecutionResultEntity?> GetResultsByScriptId(Guid scriptId, CancellationToken cancellationToken)
    {
        return await _executionResultRepository.SelectById(scriptId, cancellationToken);
    }
}