using EduAdvisor.Application.DTO.Faculties;

namespace EduAdvisor.Application.Queries.Faculties;


public sealed class GetAllFacultiesQuery
    : IRequest<Result<PaginatedList<FacultyListResponse>>>
{
    public Guid? UniversityId { get; set; }
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
