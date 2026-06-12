namespace EduAdvisor.Application.DTO.Faculties;

public sealed record FacultyResponse(
 Guid Id,
 Guid UniversityId,
 string UniversityName,
 string Name,
 string? Abbreviation,
 string? Description,
 string? Email,
 string? Website,
 string? LogoUrl,
 bool IsActive,
 int DepartmentCount,
 DateTime CreatedAt,
 DateTime? UpdatedAt
);
