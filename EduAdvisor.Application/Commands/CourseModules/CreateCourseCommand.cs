using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Application.Commands.CourseModules;

public sealed record CreateCourseCommand(
    string CourseCode,
    string CourseName,
    string? Description,
    int CreditHours,
    CourseType Type,
    int StandardLevel,
    int StandardSemester,
    Guid? DepartmentId,
    IReadOnlyCollection<Guid> PrerequisiteCourseIds)
    : IRequest<Result<Guid>>;