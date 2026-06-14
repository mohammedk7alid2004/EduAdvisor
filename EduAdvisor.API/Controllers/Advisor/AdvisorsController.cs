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
  
    [HttpGet("my-students")]
    [Authorize(Roles = "Advisor")]
    public async Task<IActionResult> GetMyStudents(
        [FromQuery] GetAdvisorStudentsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
  
    [HttpGet("student_pending")]
    public async Task<IActionResult> GetPendingRequests(
    [FromQuery] GetPendingRegistrationRequestsQuery query,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetails(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetRegistrationRequestDetailsQuery(id),
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
    [HttpPatch("{id}/rejectRegistration")]
    public async Task<IActionResult> Reject(
    Guid id,
    [FromBody] RejectRegistrationRequestCommand command,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            command with { RegistrationRequestId = id },
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
    [HttpPatch("{id}/approveRegistration")]
    public async Task<IActionResult> ApproveRegister(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ApproveRegistrationRequestCommand(id),
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}