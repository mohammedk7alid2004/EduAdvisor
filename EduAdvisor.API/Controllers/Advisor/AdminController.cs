using EduAdvisor.Application.Queries.Student;
using EduAdvisor.Application.Queries.Users;
using EduAdvisorEduAdvisor.Application.Queries.AuthModules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduAdvisor.API.Controllers.Admin;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpPost("{advisorId:guid}/assign-students")]
    public async Task<IActionResult> AssignStudents(
    Guid advisorId,
    [FromBody] AssignStudentsToAdvisorCommand command,
    CancellationToken cancellationToken)
    {
        command.AdvisorId = advisorId;


    var result = await mediator.Send(command, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPatch("approve-advisor/{id:guid}")]
    public async Task<IActionResult> ApproveAdvisor(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ApproveAdvisorCommand(id),
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("pending-advisors")]
    public async Task<IActionResult> GetPendingAdvisors(
        [FromQuery] GetPendingAdvisorsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            query,
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("students")]
    public async Task<IActionResult> GetStudents(
    [FromQuery] GetAllStudentsQuery query,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("advisors")]
    public async Task<IActionResult> GetAllAdvisors(
    [FromQuery] GetAllAdvisorsQuery query,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}
