using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Dtos.Responses;
using DockScripter.Domain.Entities;
using DockScripter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DockScripter.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnvironmentController : ControllerBase
{
    private readonly IEnvironmentService _environmentService;

    public EnvironmentController(IEnvironmentService environmentService)
    {
        _environmentService = environmentService;
    }

    [HttpPost]
    public async Task<IActionResult> InitializeEnvironment(EnvironmentRequestDTO environmentDto,
        CancellationToken cancellationToken)
    {
        var userId = ControllerUtils.GetUserIdFromToken(this);

        var environment = new EnvironmentEntity
        {
            EnvironmentName = environmentDto.EnvironmentName,
            UserId = userId
        };

        await _environmentService.InitializeEnvironmentAsync(environment, cancellationToken);
        return CreatedAtAction(nameof(GetEnvironment), new { id = environment.Id }, new EnvironmentResponseDto
        {
            Id = environment.Id,
            EnvironmentName = environment.EnvironmentName,
            Status = environment.Status.ToString(),
            CreatedAt = environment.CreatedAt
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnvironmentResponseDto>> GetEnvironment(Guid id, CancellationToken cancellationToken)
    {
        var environment = await _environmentService.GetEnvironmentByIdAsync(id, cancellationToken);
        if (environment == null)
            return NotFound();

        return new EnvironmentResponseDto
        {
            Id = environment.Id,
            EnvironmentName = environment.EnvironmentName,
            Status = environment.Status.ToString(),
            CreatedAt = environment.CreatedAt
        };
    }
}