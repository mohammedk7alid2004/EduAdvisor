namespace EduAdvisor.Application.DTO.SemesterCourses;

public sealed record SemesterCourseListDto(
    Guid SemesterId,
    string SemesterName,
    int SemesterYear,
    List<CourseItemDto> Courses);

public sealed record CourseItemDto(
    Guid Id,
    Guid CourseAcademicPlanId,
    string CourseCode,
    string CourseName,
    int CreditHours,
    int Level,
    int StandardSemester,
    string? DepartmentName,
    string? CreatedBy,
    DateTime CreatedAt);

public sealed record SemesterCourseDetailsDto(
    Guid Id,
    Guid SemesterId,
    string SemesterName,
    int SemesterYear,
    bool IsRegistrationOpen,
    Guid CourseAcademicPlanId,
    string CourseCode,
    string CourseName,
    int CreditHours,
    string CourseType,
    int Level,
    int StandardSemester,
    Guid? DepartmentId,
    string? DepartmentName,
    string? CreatedBy,
    DateTime CreatedAt,
    string? UpdatedBy,
    DateTime? UpdatedAt);

public sealed record SemesterCourseSelectDto(
    Guid Id,
    string CourseCode,
    string CourseName,
    int CreditHours,
    int Level,
    int StandardSemester,
    string? DepartmentName);