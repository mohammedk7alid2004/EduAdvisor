namespace EduAdvisor.Application.DTO.Advisor;

public sealed record AdvisorResponseDto(
    Guid Id,
    string FullName,
    string Email,
    string? ImageUrl,
    string Department,
    int StudentsCount,
    bool IsPending);