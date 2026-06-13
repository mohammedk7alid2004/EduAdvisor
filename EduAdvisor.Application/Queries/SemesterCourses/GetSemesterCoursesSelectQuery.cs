using EduAdvisor.Application.DTO.SemesterCourses;

namespace EduAdvisor.Application.Queries.SemesterCourses
{
    public sealed record GetSemesterCoursesSelectQuery(
    Guid SemesterId,
    int? Level = null,
    Guid? DepartmentId = null) : IRequest<Result<List<SemesterCourseSelectDto>>>;
}
