using Docker.DotNet;
using DockScripter.Domain.Entities;

namespace DockScripter.Services.Interfaces;

public interface IDockerClient
{
    Task<string> CreateContainerAsync(string dockerImage, string dockerContainerName,
        CancellationToken cancellationToken);

    Task<string> ExecuteScriptWithFilesAsync(string localDirectory, ScriptEntity script,
        CancellationToken cancellationToken);

    Task<MultiplexedStream> GetContainerLogsAsync(string containerId, CancellationToken cancellationToken);
    Task StartContainerAsync(string containerId, CancellationToken cancellationToken);
    Task StopContainerAsync(string containerId, CancellationToken cancellationToken);

    Task<long> GetContainerExitCodeAsync(string containerId, CancellationToken cancellationToken);
}