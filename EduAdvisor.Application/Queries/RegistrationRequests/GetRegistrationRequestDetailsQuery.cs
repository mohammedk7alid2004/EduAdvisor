using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.RegistrationRequest;

namespace EduAdvisor.Application.Queries.RegistrationRequests;

public sealed record GetRegistrationRequestDetailsQuery(
    Guid RegistrationRequestId)
    : IRequest<Result<RegistrationRequestDetailsDto>>;