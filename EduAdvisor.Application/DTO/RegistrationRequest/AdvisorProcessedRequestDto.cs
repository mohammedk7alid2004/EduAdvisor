namespace EduAdvisor.Application.DTO.RegistrationRequest;

public sealed record AdvisorProcessedRequestDto(
    Guid RegistrationRequestId,
    Guid StudentId,
    string StudentName,
    string StudentCode,
    string DepartmentName,
    Guid SemesterId,
    string Status,
    string? Notes,
    DateTime SubmittedAt,
    int EnrollmentsCount);