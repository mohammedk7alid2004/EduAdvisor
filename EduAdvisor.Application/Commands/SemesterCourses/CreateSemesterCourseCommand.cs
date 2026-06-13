namespace EduAdvisor.Application.Commands.SemesterCourses;

public sealed record CreateSemesterCourseCommand(
    Guid SemesterId,
    Guid CourseAcademicPlanId) : IRequest<Result<Guid>>;
