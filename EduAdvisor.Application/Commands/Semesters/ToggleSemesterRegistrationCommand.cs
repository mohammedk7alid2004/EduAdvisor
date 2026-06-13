namespace EduAdvisor.Application.Commands.Semesters;

public sealed record ToggleSemesterRegistrationCommand(
    Guid SemesterId) : IRequest<Result<bool>>;
