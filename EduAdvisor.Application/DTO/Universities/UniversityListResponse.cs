namespace EduAdvisor.Application.DTO.Universities;
public sealed record UniversityListResponse(
    Guid Id,
    string Name,
    string? Email,
    string? Website,
    bool IsActive,
    int FacultyCount
);

