using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.RegistrationRequest;
using EduAdvisor.Domain.Enums.University;
using MediatR;

namespace EduAdvisor.Application.Queries.RegistrationRequests;

public sealed record GetAdvisorProcessedRequestsQuery(
    EnrollmentStatus? Status = null,
    string? Search = null)
    : IRequest<Result<List<AdvisorProcessedRequestDto>>>;