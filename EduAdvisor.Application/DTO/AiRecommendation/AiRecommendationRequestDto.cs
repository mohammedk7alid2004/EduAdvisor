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
    public string StudentMajor { get; set; } = string.Empty;
    public List<AiCourseRecommendationDto> Recommendations { get; set; } = [];
}

public sealed class AiCourseRecommendationDto
{
    public string CourseCode { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public decimal ExpectedGpaImpact { get; set; }
}