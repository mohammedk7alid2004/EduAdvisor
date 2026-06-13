namespace EduAdvisor.Application.DTO.CourseAcademicPlans;

public sealed record CourseAcademicPlanListDto(
    Guid Id,
    Guid CourseId,
    string CourseCode,
    string CourseName,
    int Level,
    int StandardSemester,
    string? DepartmentName,
    string? CreatedBy,
    DateTime CreatedAt);

public sealed record CourseAcademicPlanDetailsDto(
    Guid Id,
    Guid CourseId,
    string CourseCode,
    string CourseName,
    int Level,
    int StandardSemester,
    Guid? DepartmentId,
    string? DepartmentName,
    string? CreatedBy,
    DateTime CreatedAt,
    string? UpdatedBy,
    DateTime? UpdatedAt);