using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Application.DTO.CourseRecommendation
{
    public sealed record CourseRecommendationDto
    {
        public Guid CourseId { get; init; }

        public string CourseCode { get; init; } = string.Empty;

        public string CourseName { get; init; } = string.Empty;

        public CourseDifficulty Difficulty { get; init; }

        public string Description { get; init; } = string.Empty;

        public string Reasoning { get; init; } = string.Empty;

        public decimal ExpectedGpaImpact { get; init; }
    }
}
