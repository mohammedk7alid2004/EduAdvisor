using EduAdvisor.Application.DTO.CourseAcademicPlans;

namespace EduAdvisor.Application.Queries.CourseAcademicPlans
{
    public sealed record GetAllCourseAcademicPlansQuery(
     int PageNumber = 1,
     int PageSize = 10,
     string? Search = null,
     int? Level = null,
     int? StandardSemester = null,
     Guid? DepartmentId = null) : IRequest<Result<PaginatedList<CourseAcademicPlanListDto>>>;

}
