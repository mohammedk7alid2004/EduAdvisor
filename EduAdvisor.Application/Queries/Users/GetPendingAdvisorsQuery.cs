using EduAdvisor.Application.DTO.User;

namespace EduAdvisor.Application.Queries.Users;

public sealed record GetPendingAdvisorsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null) : IRequest<Result<PaginatedList<PendingAdvisorDto>>>;