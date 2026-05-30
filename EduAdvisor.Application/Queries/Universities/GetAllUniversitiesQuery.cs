using EduAdvisor.Application.DTO.Universities;

namespace EduAdvisor.Application.Queries.Universities;

public sealed class GetAllUniversitiesQuery
   : IRequest<Result<PaginatedList<UniversityListResponse>>>
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
