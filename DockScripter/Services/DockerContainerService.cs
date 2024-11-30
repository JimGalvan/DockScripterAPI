using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Entities;
using DockScripter.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DockScripter.Domain.Enums;

namespace DockScripter.Services;

public class DockerContainerService : IDockerContainerService
{
    private readonly DockerContainerRepository _dockerContainerRepository;

    public DockerContainerService(DockerContainerRepository dockerContainerRepository)
    {
        _dockerContainerRepository = dockerContainerRepository;
    }

    public async Task<DockerContainerEntity> InitializeDockerContainerAsync(DockerContainerRequestDto dockerContainerDto,
        HttpContext httpContext, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                throw new UnauthorizedAccessException("User not authenticated"));

        var dockerContainer = new DockerContainerEntity
        {
            DockerContainerName = dockerContainerDto.DockerContainerName,
            Status = DockerContainerStatus.Initializing,
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
        dockerContainer.Status = DockerContainerStatus.Ready;

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