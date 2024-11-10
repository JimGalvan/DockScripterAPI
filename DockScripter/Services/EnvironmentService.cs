using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Entities;
using DockScripter.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DockScripter.Domain.Enums;

namespace DockScripter.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly EnvironmentRepository _environmentRepository;

    public EnvironmentService(EnvironmentRepository environmentRepository)
    {
        _environmentRepository = environmentRepository;
    }

    public async Task<EnvironmentEntity> InitializeEnvironmentAsync(EnvironmentRequestDto environmentDto,
        HttpContext httpContext, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                throw new UnauthorizedAccessException("User not authenticated"));

        var environment = new EnvironmentEntity
        {
            EnvironmentName = environmentDto.EnvironmentName,
            Status = EnvironmentStatus.Initializing,
            UserId = userId
        };

        await _environmentRepository.AddAsync(environment, cancellationToken);
        await _environmentRepository.SaveChangesAsync(cancellationToken);

        return environment;
    }

    public async Task<EnvironmentEntity?> GetEnvironmentByIdAsync(Guid environmentId,
        CancellationToken cancellationToken)
    {
        return await _environmentRepository.SelectById(environmentId, cancellationToken);
    }

    public async Task<EnvironmentEntity?> UpdateEnvironmentAsync(Guid environmentId,
        EnvironmentRequestDto environmentDto, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var environment = await _environmentRepository.SelectById(environmentId, cancellationToken);
        if (environment == null)
            return null;

        environment.EnvironmentName = environmentDto.EnvironmentName;
        environment.Status = EnvironmentStatus.Ready;

        await _environmentRepository.SaveChangesAsync(cancellationToken);

        return environment;
    }

    public async Task<bool> TerminateEnvironmentAsync(Guid environmentId, CancellationToken cancellationToken)
    {
        var deleted = await _environmentRepository.DeleteById(environmentId, cancellationToken);
        if (deleted)
            await _environmentRepository.SaveChangesAsync(cancellationToken);

        return deleted;
    }
}