namespace EduAdvisor.Application.Commands.Universities;

public sealed record DeleteUniversityCommand(Guid Id)
    : IRequest<Result<bool>>;
