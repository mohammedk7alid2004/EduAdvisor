using EduAdvisor.Application.DTO.CourseModules;

namespace EduAdvisor.Application.Queries.CourseModules;

public sealed record GetCourseByIdQuery(Guid CourseId)
    : IRequest<Result<CourseDetailsDto>>;