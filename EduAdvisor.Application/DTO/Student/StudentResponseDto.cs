namespace EduAdvisor.Application.DTO.Student;

public sealed record StudentResponseDto(
    Guid Id,
    string StudentCode,
    string FullName,
    string Email,
    string? ImageUrl,
    decimal GPA,
    int AcademicYear,
    int CompletedHours,
    string Department,
    string? Advisor);