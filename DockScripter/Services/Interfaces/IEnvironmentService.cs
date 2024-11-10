using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DockScripter.Services;

public interface IEnvironmentService
{
    Task<EnvironmentEntity> InitializeEnvironmentAsync(EnvironmentRequestDto environmentDto, HttpContext httpContext,
        CancellationToken cancellationToken);

    Task<EnvironmentEntity?> GetEnvironmentByIdAsync(Guid environmentId, CancellationToken cancellationToken);

    Task<EnvironmentEntity?> UpdateEnvironmentAsync(Guid environmentId, EnvironmentRequestDto environmentDto,
        HttpContext httpContext, CancellationToken cancellationToken);

    Task<bool> TerminateEnvironmentAsync(Guid environmentId, CancellationToken cancellationToken);
}