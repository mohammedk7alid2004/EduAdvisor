namespace EduAdvisor.Application.DTO.Semesters
{
    public sealed record SemesterDetailsDto(
      Guid Id,
      string Name,
      int Year,
      DateTime StartDate,
      DateTime EndDate,
      bool IsActive,
      bool IsRegistrationOpen,
      int StandardSemesterNumber,
      bool IsCurrentDateInSemester,
      string? CreatedBy,
      DateTime CreatedAt,
      string? UpdatedBy,
      DateTime? UpdatedAt);
    
}
