using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.User;

namespace EduAdvisor.Application.Queries.RegistrationRequests;

public sealed record GetMyRegistrationRequestsQuery
    : IRequest<Result<List<RegistrationRequestListDto>>>;