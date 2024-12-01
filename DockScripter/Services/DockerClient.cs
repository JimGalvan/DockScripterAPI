using Docker.DotNet;
using Docker.DotNet.Models;
using DockScripter.Core;
using DockScripter.Domain.Entities;
using DockScripter.Services.Interfaces;

namespace DockScripter.Services;

public class DockerClient : Interfaces.IDockerClient
{
    private readonly Docker.DotNet.DockerClient _client;
    private readonly IS3Service _s3Service;

    public DockerClient(IS3Service s3Service)
    {
        _s3Service = s3Service;
        try
        {
            _client = new DockerClientConfiguration().CreateClient();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to create Docker client. Please check if the Docker Daemon is running.", ex);
        }
    }

    public async Task<string> CreateContainerAsync(string dockerImage, string dockerContainerName,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = dockerImage,
                Name = dockerContainerName ?? string.Empty,
                HostConfig = new HostConfig
                {
                    AutoRemove = true,
                    Memory = 256 * 1024 * 1024,
                    NanoCPUs = 500000000
                }
            }, cancellationToken);

            return response.ID;
        }
        catch (DockerApiException ex)
        {
            throw new Exception(
                "Docker API error occurred while creating the container. Please check the Docker Daemon and the provided parameters.",
                ex);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("Failed to connect to the Docker Daemon. Please ensure it is running and accessible.",
                ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while creating the Docker container.", ex);
        }
    }

    public async Task<string> ExecuteScriptWithFilesAsync(string localDirectory, ScriptEntity script,
        CancellationToken cancellationToken)
    {
        if (script == null)
            throw new ArgumentNullException(nameof(script), "ScriptEntity object cannot be null.");

        var entryFilePath = script.EntryFilePath;
        var dockerContainer = script.DockerContainer;

        if (string.IsNullOrEmpty(entryFilePath))
            throw new ArgumentException("Entry file path is not found in ScriptEntity object.");

        if (dockerContainer == null)
            throw new ArgumentException("Docker container is not found in ScriptEntity object.");

        var dockerImage = dockerContainer.DockerImage;

        if (string.IsNullOrEmpty(dockerImage))
            throw new ArgumentException("Docker image is not found in DockerContainerEntity object.");

        var (image, tag) = StringUtils.ParseDockerImage(dockerImage);
        await PullPythonImageAsync(image, tag, cancellationToken);

        var response = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = dockerImage,
            // Cmd = new[] { "sh", "-c", "tail -f /dev/null" }, // Keeps the container 
            Cmd = new[] { "python", $"/app/{entryFilePath}" },
            HostConfig = new HostConfig
            {
                Mounts = new List<Mount>
                {
                    new()
                    {
                        Type = "bind",
                        Source = localDirectory,
                        Target = "/app"
                    }
                },
                AutoRemove = false,
                Memory = 256 * 1024 * 1024,
                NanoCPUs = 500000000
            }
        }, cancellationToken);

        var result =
            await _client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters(),
                cancellationToken);

        if (!result)
            throw new Exception("Failed to start the container.");

        return response.ID;
    }

    public async Task<MultiplexedStream> GetContainerLogsAsync(string containerId, CancellationToken cancellationToken)
    {
        var result = await _client.Containers.GetContainerLogsAsync(containerId, false, new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Follow = true,
            Timestamps = false
        }, cancellationToken);

        if (result == null)
            throw new Exception("Failed to get container logs.");

        return result;
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