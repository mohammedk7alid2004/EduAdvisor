using EduAdvisor.Application.Common.Abstractions;

namespace EduAdvisor.Application.Commands.RegistrationRequests;

public sealed record ApproveRegistrationRequestCommand(
    Guid RegistrationRequestId)
    : IRequest<Result<bool>>;