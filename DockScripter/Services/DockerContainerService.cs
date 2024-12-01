using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Entities;
using DockScripter.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DockScripter.Domain.Enums;
using DockScripter.Services.Interfaces;

namespace DockScripter.Services;

public class DockerContainerService : IDockerContainerService
{
    private readonly DockerContainerRepository _dockerContainerRepository;
    private readonly IDockerClient _dockerClient;

    public DockerContainerService(DockerContainerRepository dockerContainerRepository, IDockerClient dockerClient)
    {
        _dockerContainerRepository = dockerContainerRepository;
        _dockerClient = dockerClient;
    }

    public async Task<DockerContainerEntity> CreateDockerContainerAsync(
        DockerContainerRequestDto dockerContainerDto,
        HttpContext httpContext, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dockerContainerDto.DockerImage))
            throw new ArgumentException("Docker image cannot be empty");

        var userId = Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                throw new UnauthorizedAccessException("User not authenticated"));


        var dockerContainerName = dockerContainerDto.DockerContainerName ?? string.Empty;
        var dockerContainerId = await _dockerClient.CreateContainerAsync(dockerContainerDto.DockerImage,
            dockerContainerName, cancellationToken);

        if (dockerContainerId == null)
            throw new Exception("Failed to create Docker container, Docker container ID is null");

        var dockerContainer = new DockerContainerEntity
        {
            DockerImage = dockerContainerDto.DockerImage,
            Status = DockerContainerStatus.Created,
            ContainerId = dockerContainerId,
            UserId = userId
        };

        await _dockerContainerRepository.AddAsync(dockerContainer, cancellationToken);
        await _dockerContainerRepository.SaveChangesAsync(cancellationToken);

        return dockerContainer;
    }

    public async Task<DockerContainerEntity?> GetDockerContainerByIdAsync(Guid dockerContainerId,
        CancellationToken cancellationToken)
    {
        return await _dockerContainerRepository.SelectById(dockerContainerId, cancellationToken);
    }

    public async Task<DockerContainerEntity?> UpdateDockerContainerAsync(Guid dockerContainerId,
        DockerContainerRequestDto dockerContainerDto, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var dockerContainer = await _dockerContainerRepository.SelectById(dockerContainerId, cancellationToken);
        if (dockerContainer == null)
            return null;

        dockerContainer.DockerContainerName = dockerContainerDto.DockerContainerName;
        await _dockerContainerRepository.SaveChangesAsync(cancellationToken);
        return dockerContainer;
    }

    public async Task<bool> TerminateDockerContainerAsync(Guid dockerContainerId, CancellationToken cancellationToken)
    {
        var deleted = await _dockerContainerRepository.DeleteById(dockerContainerId, cancellationToken);
        if (deleted)
            await _dockerContainerRepository.SaveChangesAsync(cancellationToken);

        return deleted;
    }
}