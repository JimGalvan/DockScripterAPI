using Docker.DotNet;
using Docker.DotNet.Models;
using DockScripter.Services.Interfaces;
using Docker.DotNet.Models;
using Docker.DotNet;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DockScripter.Services;

public class DockerService
{
    private readonly DockerClient _client;
    private readonly IS3Service _s3Service;

    public DockerService(IS3Service s3Service)
    {
        _s3Service = s3Service;
        _client = new DockerClientConfiguration().CreateClient();
    }

    public async Task<string> ExecuteScriptWithFilesAsync(string localDirectory, string entryFilePath,
        CancellationToken cancellationToken)
    {
        // Pull the Python image if necessary
        await PullPythonImageAsync("python", "3.8-slim", cancellationToken);

        // Create and start a container, mounting the local directory with downloaded files
        var response = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "python:3.8-slim",
            Cmd = new[] { "python", $"/app/{entryFilePath}" }, // Run the entry script file
            HostConfig = new HostConfig
            {
                Mounts = new List<Mount>
                {
                    new Mount
                    {
                        Type = "bind",
                        Source = localDirectory,
                        Target = "/app" // Mounting the directory to /app in the container
                    }
                },
                AutoRemove = true,
                Memory = 256 * 1024 * 1024,
                NanoCPUs = 500000000
            }
        }, cancellationToken);

        await _client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters(), cancellationToken);

        return response.ID;
    }

    public async Task<MultiplexedStream> GetContainerLogsAsync(string containerId, CancellationToken cancellationToken)
    {
        return await _client.Containers.GetContainerLogsAsync(containerId, false, new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Follow = true,
            Timestamps = false
        }, cancellationToken);
    }

    private async Task PullPythonImageAsync(string image, string tag, CancellationToken cancellationToken)
    {
        try
        {
            await _client.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = image,
                    Tag = tag
                },
                null,
                new Progress<JSONMessage>(message =>
                    Console.WriteLine($"{message.Status} - {message.ProgressMessage}")),
                cancellationToken
            );

            Console.WriteLine($"Successfully pulled the {image}:{tag} image.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error pulling image: {ex.Message}");
            throw;
        }
    }

    public async Task StartContainerAsync(string containerId, CancellationToken cancellationToken)
    {
        await _client.Containers.StartContainerAsync(containerId, new ContainerStartParameters(), cancellationToken);
    }

    public async Task StopContainerAsync(string containerId, CancellationToken cancellationToken)
    {
        await _client.Containers.StopContainerAsync(containerId, new ContainerStopParameters(), cancellationToken);
    }
}