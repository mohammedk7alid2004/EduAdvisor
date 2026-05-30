namespace EduAdvisor.Application.Commands.Faculties;

public sealed record DeleteFacultyCommand(Guid Id)
  : IRequest<Result<bool>>;
