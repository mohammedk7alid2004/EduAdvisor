using EduAdvisor.Application.DTO.CourseAcademicPlans;

namespace EduAdvisor.Application.Queries.CourseAcademicPlans
{

    public sealed record GetCourseAcademicPlanByIdQuery(
        Guid Id) : IRequest<Result<CourseAcademicPlanDetailsDto>>;
}
