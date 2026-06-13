namespace EduAdvisor.Application.Commands.Semesters
{
    public sealed record CreateSemesterCommand(
     string Name,
     int Year,
     DateTime StartDate,
     DateTime EndDate,
     int StandardSemesterNumber) : IRequest<Result<Guid>>;
}
