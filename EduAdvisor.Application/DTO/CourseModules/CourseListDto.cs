namespace EduAdvisor.Application.DTO.CourseListDto;

public sealed record CourseListDto(
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
    string? CreatedBy,
    DateTime CreatedAt,
    string? UpdatedBy,
    DateTime? UpdatedAt);