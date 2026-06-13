using EduAdvisor.Application.Commands.CourseAcademicPlans;
using EduAdvisor.Application.Queries.CourseAcademicPlans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduAdvisor.API.Controllers.Course;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CourseAcademicPlansController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCourseAcademicPlanCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCourseAcademicPlanCommand command,
        CancellationToken cancellationToken)
    {
        command = command with { Id = id };
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteCourseAcademicPlansCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetCourseAcademicPlanByIdQuery(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllCourseAcademicPlansQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}