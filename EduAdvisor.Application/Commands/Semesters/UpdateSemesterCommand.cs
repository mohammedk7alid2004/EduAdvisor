namespace EduAdvisor.Application.Commands.Semesters
{
    public sealed record UpdateSemesterCommand(
     Guid SemesterId,
     string Name,
     int Year,
     DateTime StartDate,
     DateTime EndDate,
     int StandardSemesterNumber) : IRequest<Result<bool>>;
}
