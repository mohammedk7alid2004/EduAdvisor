using EduAdvisor.Application.Commands.Faculties;

using EduAdvisor.Application.Queries.Faculties;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduAdvisor.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FacultiesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFacultyCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateFacultyCommand command,
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
            new DeleteFacultyCommand(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPatch("toggle-status/{id}")]
    public async Task<IActionResult> ToggleStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ToggleFacultyStatusCommand(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetFacultyByIdQuery(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllFacultiesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}