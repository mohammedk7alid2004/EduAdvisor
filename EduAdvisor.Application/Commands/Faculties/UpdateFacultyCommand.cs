using EduAdvisor.Application.DTO.Faculties;

namespace EduAdvisor.Application.Commands.Faculties;

public sealed class UpdateFacultyCommand : IRequest<Result<FacultyResponse>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Abbreviation { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
}
