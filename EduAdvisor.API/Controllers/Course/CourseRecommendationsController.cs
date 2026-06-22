using EduAdvisor.Application.Commands.GenerateRecommendations;
using EduAdvisor.Application.Queries.CourseModules;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EduAdvisor.API.Controllers.Course;

[ApiController]
[Route("api/[controller]")]
public sealed class CourseRecommendationsController(
    IMediator mediator) : ControllerBase
{
    [HttpPost("student")]
    public async Task<IActionResult> GetByStudent(
        [FromBody] GetStudentRecommendationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}