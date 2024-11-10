using DockScripter.Domain.Dtos.Requests;
using DockScripter.Domain.Dtos.Responses;
using DockScripter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DockScripter.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class EnvironmentController : ControllerBase
{
    private readonly IEnvironmentService _environmentService;

    public EnvironmentController(IEnvironmentService environmentService)
    {
        _environmentService = environmentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEnvironment(EnvironmentRequestDto environmentDto,
        CancellationToken cancellationToken)
    {
        var createdEnvironment =
            await _environmentService.InitializeEnvironmentAsync(environmentDto, HttpContext, cancellationToken);

        return CreatedAtAction(nameof(GetEnvironment), new { id = createdEnvironment.Id }, new EnvironmentResponseDto
        {
            Id = createdEnvironment.Id,
            EnvironmentName = createdEnvironment.EnvironmentName,
            Status = createdEnvironment.Status.ToString(),
            CreatedAt = createdEnvironment.CreationDateTimeUtc
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
            CreatedAt = environment.CreationDateTimeUtc
        };
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEnvironment(Guid id, EnvironmentRequestDto environmentDto,
        CancellationToken cancellationToken)
    {
        var updatedEnvironment =
            await _environmentService.UpdateEnvironmentAsync(id, environmentDto, HttpContext, cancellationToken);
        if (updatedEnvironment == null)
            return NotFound();

        return Ok(new EnvironmentResponseDto
        {
            Id = updatedEnvironment.Id,
            EnvironmentName = updatedEnvironment.EnvironmentName,
            Status = updatedEnvironment.Status.ToString(),
            CreatedAt = updatedEnvironment.CreationDateTimeUtc
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEnvironment(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _environmentService.TerminateEnvironmentAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}