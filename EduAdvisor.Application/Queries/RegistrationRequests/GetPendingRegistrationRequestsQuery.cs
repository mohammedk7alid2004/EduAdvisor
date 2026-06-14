using EduAdvisor.Application.DTO.RegistrationRequest;

namespace EduAdvisor.Application.Queries.RegistrationRequests;

public sealed record GetPendingRegistrationRequestsQuery(
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<PendingRegistrationRequestDto>>>;