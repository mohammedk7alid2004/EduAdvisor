namespace EduAdvisor.Application.Commands.Universities;

public sealed record ToggleUniversityStatusCommand(Guid Id)
    : IRequest<Result<bool>>;
