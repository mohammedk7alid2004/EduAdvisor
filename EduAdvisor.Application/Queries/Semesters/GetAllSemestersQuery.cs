using EduAdvisor.Application.DTO.Semesters;

namespace EduAdvisor.Application.Queries.Semesters
{
    public sealed record GetAllSemestersQuery(
     int PageNumber = 1,
     int PageSize = 10,
     string? Search = null,
     bool? IsActive = null,
     bool? IsRegistrationOpen = null,
     int? StandardSemesterNumber = null) : IRequest<Result<PaginatedList<SemesterListDto>>>;

}
