using EduAdvisor.Application.Commands.RegistrationRequests;
using EduAdvisor.Application.Queries.RegistrationRequests;
using EduAdvisor.Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduAdvisor.API.Controllers.Advisor;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AdvisorsController(IMediator mediator) : ControllerBase
{
    [HttpPost("{advisorId}/assign-students")]
    public async Task<IActionResult> AssignStudents(
        Guid advisorId,
        [FromBody] AssignStudentsToAdvisorCommand command,
        CancellationToken cancellationToken)
    {
        command.AdvisorId = advisorId;
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("my-students")]
    [Authorize(Roles = "Advisor")]
    public async Task<IActionResult> GetMyStudents(
        [FromQuery] GetAdvisorStudentsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
    [HttpPatch("approve/{id}")]
    public async Task<IActionResult> Approve(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ApproveRegistrationRequestCommand(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
    [HttpPatch("reject/{id:guid}")]
    public async Task<IActionResult> Reject(
    Guid id,
    [FromBody] RejectRegistrationRequestCommand command,
    CancellationToken cancellationToken)
    {
        command = command with
        {
            RegistrationRequestId = id
        };

        var result = await mediator.Send(command, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(
    [FromQuery] GetPendingRegistrationRequestsQuery query,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("RegistrationRequest/{id:guid}")]
    [Authorize(Roles = "Advisor")]
    public async Task<IActionResult> GetDetails(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetRegistrationRequestDetailsQuery(id),
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

}