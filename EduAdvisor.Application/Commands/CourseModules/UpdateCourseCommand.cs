// Command
using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Application.Commands.CourseModules;

public sealed record UpdateCourseCommand(
    Guid CourseId,
    string CourseName,
    string? Description,
    int CreditHours,
    CourseType Type,
    int StandardLevel,
    int StandardSemester,
    Guid? DepartmentId,
    List<Guid> PrerequisiteCourseIds) : IRequest<Result<bool>>;