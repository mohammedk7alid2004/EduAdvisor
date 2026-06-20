namespace EduAdvisor.Application.DTO.User;

public record CurrentUserResponseDTO(
    string Id,
    string FullName,
    string Email,
    string? Phone,
    string? ProfileImageUrl,
    bool IsVerified,
    DateTime CreatedAt,
    string Role,
    StudentProfileDto? StudentProfile,
    AdvisorProfileDto? AdvisorProfile);

public record StudentProfileDto(
    Guid Id,
    string StudentCode,
    string DepartmentName,
    decimal GPA,
    int CompletedHours,
    int AcademicYear,
    string? AdvisorName);

public record AdvisorProfileDto(
    Guid Id,
    string DepartmentName,
    bool IsPending,
    int StudentsCount,
    int PendingRequestsCount);