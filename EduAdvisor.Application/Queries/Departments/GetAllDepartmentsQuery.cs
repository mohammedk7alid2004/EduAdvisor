using EduAdvisor.Application.DTO.Departments;

namespace EduAdvisor.Application.Queries.Departments;

public sealed record GetAllDepartmentsQuery(
    string? Search,
    Guid? FacultyId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedList<DescriptionListResponse>>>;