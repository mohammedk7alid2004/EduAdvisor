using EduAdvisor.Application.Commands.Semesters;
using EduAdvisor.Application.Queries.Semesters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduAdvisor.API.Controllers.Semesters;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SemestersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSemesterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSemesterCommand command,
        CancellationToken cancellationToken)
    {
        command = command with { SemesterId = id };
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteSemestersCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPatch("toggle-activation/{id}")]
    public async Task<IActionResult> ToggleActivation(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ToggleSemesterActivationCommand(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPatch("toggle-registration/{id}")]
    public async Task<IActionResult> ToggleRegistration(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ToggleSemesterRegistrationCommand(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetSemesterByIdQuery(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllSemestersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}