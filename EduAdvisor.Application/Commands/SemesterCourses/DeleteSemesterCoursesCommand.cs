namespace EduAdvisor.Application.Commands.SemesterCourses
{
    public sealed record DeleteSemesterCoursesCommand(
       List<Guid> Ids) : IRequest<Result<bool>>;
}
