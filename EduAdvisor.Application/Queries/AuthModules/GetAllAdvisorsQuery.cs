using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.Advisor;
using MediatR;

namespace EduAdvisorEduAdvisor.Application.Queries.AuthModules;

public sealed record GetAllAdvisorsQuery(
    string? Search = null,
    Guid? DepartmentId = null,
    bool? IsPending = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<AdvisorResponseDto>>>;