using EduAdvisor.Application.DTO.Universities;

namespace EduAdvisor.Application.Commands.Universities;

public sealed record CreateUniversityCommand(
    string Name,
    string? Description,
    string? Address,
    string? Email,
    string? Website,
    string? PhoneNumber)
    : IRequest<Result<UniversityResponse>>;
