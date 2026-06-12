using EduAdvisor.Application.Commands.CourseModules;
using EduAdvisor.Application.Queries.CourseModules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduAdvisor.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CoursesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCourseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCourseCommand command,
        CancellationToken cancellationToken)
    {
        command = command with { CourseId = id };
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteCoursesCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPatch("toggle-status/{id}")]
    public async Task<IActionResult> ToggleStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ToggleCourseStatusCommand(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetCourseByIdQuery(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllCoursesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}