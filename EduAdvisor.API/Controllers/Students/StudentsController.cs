using EduAdvisor.Application.Commands.Student;
using EduAdvisor.Application.Queries.CourseModules;
using EduAdvisor.Application.Queries.RegistrationRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduAdvisor.API.Controllers.Students;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Student")]
public class StudentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("available-courses")]
    public async Task<IActionResult> GetAvailableCourses(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAvailableCoursesForStudentQuery(),
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("registration-requests")]
    public async Task<IActionResult> SubmitRegistrationRequest(
        [FromBody] SubmitRegistrationRequestCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("registration-requests")]
    public async Task<IActionResult> GetMyRegistrationRequests(
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetMyRegistrationRequestsQuery(),
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}