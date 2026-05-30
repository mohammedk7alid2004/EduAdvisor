namespace EduAdvisor.Application.DTO.Faculties;

public sealed record FacultyListResponse(
    Guid Id,
    Guid UniversityId,
    string UniversityName,
    string Name,
    string? Abbreviation,
    string? Email,
    bool IsActive,
    int DepartmentCount
);
