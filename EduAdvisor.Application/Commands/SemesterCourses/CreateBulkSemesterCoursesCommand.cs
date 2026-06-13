namespace EduAdvisor.Application.Commands.SemesterCourses
{
    public sealed record CreateBulkSemesterCoursesCommand(
      Guid SemesterId,
      List<Guid> CourseAcademicPlanIds) : IRequest<Result<bool>>;
}
