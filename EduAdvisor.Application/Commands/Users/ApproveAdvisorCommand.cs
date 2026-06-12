namespace EduAdvisor.Application.Commands.Users;

public sealed record ApproveAdvisorCommand(Guid AdvisorId)
    : IRequest<Result<bool>>;