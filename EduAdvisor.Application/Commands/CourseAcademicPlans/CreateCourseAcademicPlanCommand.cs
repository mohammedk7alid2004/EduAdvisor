namespace EduAdvisor.Application.Commands.CourseAcademicPlans
{
    public sealed record CreateCourseAcademicPlanCommand(
      Guid CourseId,
      int Level,
      int StandardSemester,
      Guid? DepartmentId) : IRequest<Result<Guid>>;
}
