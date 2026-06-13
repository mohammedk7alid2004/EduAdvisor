namespace EduAdvisor.Application.Commands.CourseAcademicPlans
{
    public sealed record UpdateCourseAcademicPlanCommand(
     Guid Id,
     Guid CourseId,
     int Level,
     int StandardSemester,
     Guid? DepartmentId) : IRequest<Result<bool>>;
}
