using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockScripter.Services;

public class DockerService
{
    private readonly DockerClient _client;

    public DockerService()
    {
        // _client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
        _client = new DockerClientConfiguration().CreateClient();
    }

    public async Task<string> CreateContainerAsync(string scriptFilePath, CancellationToken cancellationToken)
    {
        IList<ContainerListResponse> containers = await _client.Containers.ListContainersAsync(
            new ContainersListParameters()
            {
                Limit = 10,
            }, cancellationToken);

        await PullPythonImageAsync("python", "3.8-slim", cancellationToken);

        var response = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "python:3.8-slim",
            HostConfig = new HostConfig()
            {
                DNS = new[] { "8.8.8.8", "8.8.4.4" }
            }
            // Cmd = new[] { "python", "/scripts/test_script.py" },
            // HostConfig = new HostConfig
            // {
            //     Mounts = new List<Mount>
            //     {
            //         new Mount
            //         {
            //             Type = "bind",
            //             Source = scriptFilePath,
            //             Target = "/scripts/test_script.py"
            //         }
            //     },
            //     Memory = 256 * 1024 * 1024,
            //     NanoCPUs = 500000000
            // }
        }, cancellationToken);

        return response.ID;
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