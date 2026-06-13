using EduAdvisor.Application.Queries.CourseModules;
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
}