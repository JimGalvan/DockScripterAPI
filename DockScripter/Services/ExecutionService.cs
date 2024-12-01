using DockScripter.Domain.Entities;
using DockScripter.Domain.Enums;
using DockScripter.Repositories;
using DockScripter.Services;
using DockScripter.Services.Interfaces;

public class ExecutionService : IExecutionService
{
    private readonly IDockerClient _dockerClient;
    private readonly IS3Service _s3Service;
    private readonly ExecutionResultRepository _executionResultRepository;

    public ExecutionService(IDockerClient dockerClient, IS3Service s3Service,
        ExecutionResultRepository executionResultRepository)
    {
        _dockerClient = dockerClient;
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
        var containerId = string.Empty;

        // If Script object doesn't have any files, return immediately
        if (script.Files.Count == 0)
        {
            result.Status = ExecutionStatus.Failed;
            result.ErrorOutput = "Script does not have any files associated with it.";
            await _executionResultRepository.AddAsync(result, cancellationToken);
            await _executionResultRepository.SaveChangesAsync(cancellationToken);
            return result;
        }

        try
        {
            // Step 1: Download all files associated with the script from S3
            foreach (var scriptFile in script.Files)
            {
                var filePath = Path.Combine(tempDirectory, Path.GetFileName(scriptFile.S3Key));
                await using (var downloadStream = await _s3Service.DownloadFileAsync(scriptFile.S3Key))
                await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await downloadStream.CopyToAsync(fileStream, cancellationToken);
                }
            }

            // Step 2: Capture output and status

            // Create paths for stdout and stderr log files
            var outputFilePath = Path.Combine(tempDirectory, "stdout.log");
            var errorFilePath = Path.Combine(tempDirectory, "stderr.log");

            // Open file streams for output and error logging
            await using var outputFileStream = new StreamWriter(outputFilePath);
            await using var errorFileStream = new StreamWriter(errorFilePath);

            containerId =
                await _dockerClient.ExecuteScriptWithFilesAsync(tempDirectory, script,
                    cancellationToken);

            using var logStream = await _dockerClient.GetContainerLogsAsync(containerId, cancellationToken);
            using var memoryStream = new MemoryStream();
            await logStream.CopyFromAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream);

            // Write log lines directly to files
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line != null)
                {
                    if (line.Contains("ERROR"))
                    {
                        await errorFileStream.WriteLineAsync(line);
                    }
                    else
                    {
                        await outputFileStream.WriteLineAsync(line);
                    }
                }
            }

            // Step 3: Upload logs to S3
            var outputS3Key = $"logs/{script.Id}/stdout.log";
            var errorS3Key = $"logs/{script.Id}/stderr.log";

            var outputS3Path = await _s3Service.UploadLogFileAsync(outputFilePath, outputS3Key);
            var errorS3Path = await _s3Service.UploadLogFileAsync(errorFilePath, errorS3Key);

            // Store S3 paths in ExecutionResultEntity
            result.Status = ExecutionStatus.Success;
            result.OutputFilePath = outputS3Key;
            result.ErrorOutputFilePath = errorS3Key;
        }
        catch (Exception ex)
        {
            result.Status = ExecutionStatus.Failed;
            result.ErrorOutput = ex.Message;
        }
        finally
        {
            // Cleanup the temporary directory and local log files after uploading to S3
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);

            // Cleanup Docker container
            if (!string.IsNullOrEmpty(containerId))
                await _dockerClient.StopContainerAsync(containerId, cancellationToken);
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