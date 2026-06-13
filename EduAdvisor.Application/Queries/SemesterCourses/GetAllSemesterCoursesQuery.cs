using EduAdvisor.Application.DTO.SemesterCourses;

namespace EduAdvisor.Application.Queries.SemesterCourses
{
    public sealed record GetAllSemesterCoursesQuery(
     int PageNumber = 1,
     int PageSize = 10,
     string? Search = null,
     Guid? SemesterId = null,
     Guid? DepartmentId = null,
     int? Level = null,
     int? StandardSemester = null) : IRequest<Result<PaginatedList<SemesterCourseListDto>>>;

}
