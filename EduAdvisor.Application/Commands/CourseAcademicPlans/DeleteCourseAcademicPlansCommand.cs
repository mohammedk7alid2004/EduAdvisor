namespace EduAdvisor.Application.Commands.CourseAcademicPlans
{
    public sealed record DeleteCourseAcademicPlansCommand(
      List<Guid> Ids) : IRequest<Result<bool>>;
}
