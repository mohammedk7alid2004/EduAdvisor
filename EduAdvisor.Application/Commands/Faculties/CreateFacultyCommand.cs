using EduAdvisor.Application.DTO.Faculties;

namespace EduAdvisor.Application.Commands.Faculties;

public sealed record CreateFacultyCommand(
    Guid UniversityId,
    string Name,
    string? Abbreviation,
    string? Email,
    string? Website,
    string? Description)
    : IRequest<Result<FacultyResponse>>;
