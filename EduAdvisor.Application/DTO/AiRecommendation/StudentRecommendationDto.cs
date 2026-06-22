using EduAdvisor.Application.Commands.GenerateRecommendations;

namespace EduAdvisor.Application.DTO.AiRecommendation;

public sealed class StudentRecommendationDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int CreditHours { get; set; }
    public Guid SemesterId { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public decimal ExpectedGpaImpact { get; set; }
}
public sealed class AvailableCourseDetailsDto
{
    public Guid SemesterCourseId { get; set; }
    public Guid CourseId { get; set; }
    public Guid SemesterId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int CreditHours { get; set; }
    public bool IsRegistered { get; set; }
}

public sealed record StudentRecommendationsResultDto(
    bool HasAiRecommendations,
    List<StudentRecommendationDto> Recommendations,
    List<AvailableCourseDetailsDto> AvailableCourses);