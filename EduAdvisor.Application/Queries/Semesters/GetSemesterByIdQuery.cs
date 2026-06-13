using EduAdvisor.Application.DTO.Semesters;

namespace EduAdvisor.Application.Queries.Semesters
{
    public sealed record GetSemesterByIdQuery(
        Guid SemesterId) : IRequest<Result<SemesterDetailsDto>>;
}
