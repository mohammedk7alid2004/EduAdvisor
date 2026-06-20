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
    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudent(
        [FromRoute] Guid studentId,
        [FromQuery] Guid semesterId,
        [FromQuery] string studentMajor,
        [FromQuery] decimal currentGpa,
        [FromQuery] int level,
        [FromQuery] int completedHours,
        [FromQuery] int registeredHours,
        [FromQuery] int semester,
        [FromQuery] bool isGraduationSemester,
        [FromQuery] List<AvailableCourseDto> availableCourses,
        CancellationToken cancellationToken)
    {
        var query = new GetStudentRecommendationsQuery(
            StudentId: studentId,
            SemesterId: semesterId,
            StudentMajor: studentMajor,
            CurrentGpa: currentGpa,
            Level: level,
            CompletedHours: completedHours,
            RegisteredHours: registeredHours,
            Semester: semester,
            IsGraduationSemester: isGraduationSemester,
            AvailableCourses: availableCourses);

        var result = await mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}