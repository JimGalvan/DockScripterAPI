using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Entities;

namespace DockScripter.Services;

public interface IDockerContainerService
{
    Task<DockerContainerEntity> StoreDockerContainerDataAsync(DockerContainerRequestDto dockerContainerDto,
        HttpContext httpContext,
        CancellationToken cancellationToken);

    Task<DockerContainerEntity?> GetDockerContainerByIdAsync(Guid dockerContainerId, CancellationToken cancellationToken);

    Task<DockerContainerEntity?> UpdateDockerContainerAsync(Guid dockerContainerId, DockerContainerRequestDto dockerContainerDto,
        HttpContext httpContext, CancellationToken cancellationToken);

    Task<bool> TerminateDockerContainerAsync(Guid dockerContainerId, CancellationToken cancellationToken);
}