using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockScripter.Services;

public class DockerService
{
    private readonly DockerClient _client;

    public DockerService()
    {
        _client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
    }

    public async Task<string> CreateContainerAsync(string scriptFilePath, CancellationToken cancellationToken)
    {
        var response = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "python:3.8", // use a Python image or similar for other languages
            Cmd = new[] { "python", "/scripts/test_script.py" }, // assuming the script is mounted in /scripts
            HostConfig = new HostConfig
            {
                Mounts = new List<Mount>
                {
                    new Mount
                    {
                        Type = "bind",
                        Source = scriptFilePath,
                        Target = "/scripts/test_script.py"
                    }
                },
                Memory = 256 * 1024 * 1024, // 256 MB memory limit
                NanoCPUs = 500000000 // Limit to 0.5 CPU
            }
        }, cancellationToken);

        return response.ID;
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