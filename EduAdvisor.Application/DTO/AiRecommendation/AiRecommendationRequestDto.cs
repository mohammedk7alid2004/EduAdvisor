using System.Text.Json.Serialization;

namespace EduAdvisor.Application.DTO.AiRecommendation;

public sealed class AiRecommendationRequestDto
{
    public string StudentMajor { get; set; } = string.Empty;
    public decimal CurrentGpa { get; set; }
    public int Level { get; set; }
    public int CompletedHours { get; set; }
    public int RegisteredHours { get; set; }
    public int Semester { get; set; }
    public bool IsGraduationSemester { get; set; }
    public List<AiAvailableCourseDto> AvailableCourses { get; set; } = [];
}

public sealed class AiAvailableCourseDto
{
    public string CourseCode { get; set; } = string.Empty;
    public List<int> PrereqGrades { get; set; } = [];
}

public sealed class AiRecommendationResponseDto
{
    public decimal StudentGpa { get; set; }
    public string StudentMajor { get; set; } = string.Empty;
    public int Level { get; set; }
    public bool OnProbation { get; set; }
    public AiRegistrationLimitsDto RegistrationLimits { get; set; } = new();
    public decimal ProjectedSemesterGpa { get; set; }
    public string OverallRating { get; set; } = string.Empty;

    [JsonPropertyName("recommended_courses")]
    public List<AiCourseRecommendationDto> Recommendations { get; set; } = [];
}

public sealed class AiRegistrationLimitsDto
{
    public int MinHours { get; set; }
    public int MaxHours { get; set; }
    public bool OnProbation { get; set; }
    public string Note { get; set; } = string.Empty;
}

public sealed class AiCourseRecommendationDto
{
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int CourseCredits { get; set; }
    public int CourseLevel { get; set; }

    public decimal Difficulty { get; set; }
    public string DifficultyLabel { get; set; } = string.Empty;

    public decimal PredictedScore { get; set; }
    public decimal PredictedGpa { get; set; }
    public string PredictedLetter { get; set; } = string.Empty;

    public bool IsPassing { get; set; }
    public bool IsGpaBooster { get; set; }

    public AiCourseAdviceDto Advice { get; set; } = new();
}

public sealed class AiCourseAdviceDto
{
    public string Verdict { get; set; } = string.Empty;
    public List<string> Reasons { get; set; } = [];
}