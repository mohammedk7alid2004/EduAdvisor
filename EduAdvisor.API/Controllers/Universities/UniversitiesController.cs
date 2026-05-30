using EduAdvisor.Application.Commands.Universities;
using EduAdvisor.Application.Queries.Universities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduAdvisor.API.Controllers.Universities;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UniversitiesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUniversityCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUniversityCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new DeleteUniversityCommand(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPatch("toggle-status/{id}")]
    public async Task<IActionResult> ToggleStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ToggleUniversityStatusCommand(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetUniversityByIdQuery(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllUniversitiesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}