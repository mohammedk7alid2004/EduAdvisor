namespace EduAdvisor.Application.DTO.CourseModules;

public sealed record CourseSelectDto(
    Guid Id,
    string CourseCode,
    string CourseName,
    int CreditHours,
    string Type,
    int StandardLevel,
    int StandardSemester);