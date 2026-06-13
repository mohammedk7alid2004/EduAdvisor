using EduAdvisor.Application.DTO.CourseModules;

namespace EduAdvisor.Application.Queries.CourseModules;

public sealed record GetCoursesSelectQuery(
    Guid? DepartmentId = null,
    int? StandardLevel = null,
    int? StandardSemester = null,
    string? CourseType = null) : IRequest<Result<List<CourseSelectDto>>>;