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
public class DockerContainerController : ControllerBase
{
    private readonly IDockerContainerService _dockerContainerService;

    public DockerContainerController(IDockerContainerService dockerContainerService)
    {
        _dockerContainerService = dockerContainerService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDockerContainer(DockerContainerRequestDto dockerContainerDto,
        CancellationToken cancellationToken)
    {
        var createdDockerContainer =
            await _dockerContainerService.InitializeDockerContainerAsync(dockerContainerDto, HttpContext, cancellationToken);

        return CreatedAtAction(nameof(GetDockerContainer), new { id = createdDockerContainer.Id }, new DockerContainerResponseDto
        {
            Id = createdDockerContainer.Id,
            DockerContainer = createdDockerContainer.DockerContainerName,
            Status = createdDockerContainer.Status.ToString(),
            CreatedAt = createdDockerContainer.CreationDateTimeUtc
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DockerContainerResponseDto>> GetDockerContainer(Guid id, CancellationToken cancellationToken)
    {
        var dockerContainer = await _dockerContainerService.GetDockerContainerByIdAsync(id, cancellationToken);
        if (dockerContainer == null)
            return NotFound();

        return new DockerContainerResponseDto
        {
            Id = dockerContainer.Id,
            DockerContainer = dockerContainer.DockerContainerName,
            Status = dockerContainer.Status.ToString(),
            CreatedAt = dockerContainer.CreationDateTimeUtc
        };
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDockerContainer(Guid id, DockerContainerRequestDto dockerContainerDto,
        CancellationToken cancellationToken)
    {
        var updatedDockerContainer =
            await _dockerContainerService.UpdateDockerContainerAsync(id, dockerContainerDto, HttpContext, cancellationToken);
        if (updatedDockerContainer == null)
            return NotFound();

        return Ok(new DockerContainerResponseDto
        {
            Id = updatedDockerContainer.Id,
            DockerContainer = updatedDockerContainer.DockerContainerName,
            Status = updatedDockerContainer.Status.ToString(),
            CreatedAt = updatedDockerContainer.CreationDateTimeUtc
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDockerContainer(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _dockerContainerService.TerminateDockerContainerAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}