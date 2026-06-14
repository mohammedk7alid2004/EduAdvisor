using EduAdvisor.Application.Common.Abstractions;

namespace EduAdvisor.Application.Commands.RegistrationRequests;

public sealed record RejectRegistrationRequestCommand(
    Guid RegistrationRequestId,
    string Reason)
    : IRequest<Result<bool>>;