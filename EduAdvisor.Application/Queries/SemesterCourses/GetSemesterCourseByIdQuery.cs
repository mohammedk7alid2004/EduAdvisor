using EduAdvisor.Application.DTO.SemesterCourses;

namespace EduAdvisor.Application.Queries.SemesterCourses
{

    public sealed record GetSemesterCourseByIdQuery(
        Guid Id) : IRequest<Result<SemesterCourseDetailsDto>>;
}
