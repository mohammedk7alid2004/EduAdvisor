using EduAdvisor.Application.DTO.Universities;

namespace EduAdvisor.Application.Commands.Universities;

public sealed class UpdateUniversityCommand : IRequest<Result<UniversityResponse>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}
