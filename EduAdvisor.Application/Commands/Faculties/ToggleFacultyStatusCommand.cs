namespace EduAdvisor.Application.Commands.Faculties;

public sealed record ToggleFacultyStatusCommand(Guid Id)
 : IRequest<Result<bool>>;
