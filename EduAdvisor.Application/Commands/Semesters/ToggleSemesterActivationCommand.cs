namespace EduAdvisor.Application.Commands.Semesters
{
    public sealed record ToggleSemesterActivationCommand(
     Guid SemesterId) : IRequest<Result<bool>>;
}
