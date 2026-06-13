namespace EduAdvisor.Application.Commands.Semesters
{
    public sealed record DeleteSemestersCommand(
     List<Guid> SemesterIds) : IRequest<Result<bool>>;
}
