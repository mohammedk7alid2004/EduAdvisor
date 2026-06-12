namespace EduAdvisor.Application.DTO.CourseModules;

public sealed record CourseDetailsDto(
    Guid Id,
    string CourseCode,
    string CourseName,
    string? Description,
    int CreditHours,
    string Type,
    int StandardLevel,
    int StandardSemester,
    string? DepartmentName,
    bool IsDeleted,
    DateTime? DeletedAt,
    string? DeletedBy,
    string? CreatedBy,
    DateTime CreatedAt,
    string? UpdatedBy,
    DateTime? UpdatedAt,
    IEnumerable<CoursePrerequisiteDto> Prerequisites);

public sealed record CoursePrerequisiteDto(
    Guid CourseId,
    string CourseCode,
    string CourseName);