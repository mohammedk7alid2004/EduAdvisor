namespace EduAdvisor.Application.DTO.CourseModules;

public sealed record AvailableCourseDto
{
    public Guid SemesterCourseId { get; init; }

    public Guid CourseId { get; init; }

    public string CourseCode { get; init; } = string.Empty;

    public string CourseName { get; init; } = string.Empty;

    public int CreditHours { get; init; }

    public bool IsRetake { get; init; }
}
public sealed record CourseCandidateDto
{
    public Guid SemesterCourseId { get; init; }

    public Guid CourseId { get; init; }

    public string CourseCode { get; init; } = string.Empty;

    public string CourseName { get; init; } = string.Empty;

    public int CreditHours { get; init; }

    public List<Guid> PrerequisiteIds { get; init; } = [];
}