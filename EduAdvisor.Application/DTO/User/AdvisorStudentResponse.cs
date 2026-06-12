namespace EduAdvisor.Application.DTO.User;

public sealed record AdvisorStudentResponse(
    Guid Id,
    string StudentCode,
    string FullName,
    string Email,
    decimal GPA,
    int AcademicYear
);