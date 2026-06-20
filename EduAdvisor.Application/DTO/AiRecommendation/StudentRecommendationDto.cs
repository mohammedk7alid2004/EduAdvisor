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