namespace EduAdvisor.Application.DTO.Universities;

public sealed record UniversityResponse(
 Guid Id,
 string Name,
 string? Description,
 string? Address,
 string? Email,
 string? Website,
 string? PhoneNumber,
 bool IsActive,
 int FacultyCount,
 DateTime CreatedAt,
 DateTime? UpdatedAt
);
