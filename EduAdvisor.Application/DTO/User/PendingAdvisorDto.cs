namespace EduAdvisor.Application.DTO.User;

public sealed record PendingAdvisorDto(
    Guid Id,
    string FullName,
    string Email,
    string DepartmentName,
    DateTime CreatedAt);