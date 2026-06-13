namespace EduAdvisor.Application.DTO.Semesters
{
    public sealed record SemesterListDto(
        Guid Id,
        string Name,
        int Year,
        DateTime StartDate,
        DateTime EndDate,
        bool IsActive,
        bool IsRegistrationOpen,
        int StandardSemesterNumber,
        string? CreatedBy,
        DateTime CreatedAt);
}
