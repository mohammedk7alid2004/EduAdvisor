using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.CourseModules;

namespace EduAdvisor.Application.Queries.CourseModules;

public sealed record GetAvailableCoursesForStudentQuery
    : IRequest<Result<List<AvailableCourseDto>>>;