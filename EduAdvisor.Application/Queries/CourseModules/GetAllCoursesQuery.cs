using EduAdvisor.Application.DTO.CourseListDto;

namespace EduAdvisor.Application.Queries.CourseModules;

public sealed record GetAllCoursesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    bool? IsDeleted = null) : IRequest<Result<PaginatedList<CourseListDto>>>;