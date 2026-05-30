namespace EduAdvisor.Application.DTO.Departments;

public sealed record DescriptionListResponse
(
    Guid Id,
    Guid FacultyId,
    string FacultyName,
    string Name,
    string Description,
    string Code,
    string CreatedBy,
    string UpdatedBy,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);