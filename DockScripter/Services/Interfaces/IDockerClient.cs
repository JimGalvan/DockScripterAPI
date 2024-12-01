using Docker.DotNet;

namespace DockScripter.Services.Interfaces;

public interface IDockerClient
{
    Task<string> CreateContainerAsync(string dockerImage, string dockerContainerName,
        CancellationToken cancellationToken);

    Task<string> ExecuteScriptWithFilesAsync(string localDirectory, string entryFilePath,
        CancellationToken cancellationToken);

    Task<MultiplexedStream> GetContainerLogsAsync(string containerId, CancellationToken cancellationToken);
    Task StartContainerAsync(string containerId, CancellationToken cancellationToken);
    Task StopContainerAsync(string containerId, CancellationToken cancellationToken);
}