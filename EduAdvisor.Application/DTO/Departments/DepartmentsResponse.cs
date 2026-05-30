namespace EduAdvisor.Application.DTO.Departments;

public sealed record DepartmentsResponse
(
    Guid Id,
    Guid FacultyId,
    string FacultyName,
    string Name,
    string? Code,
    string? Description,
    bool IsActive,
    int SubjectCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
