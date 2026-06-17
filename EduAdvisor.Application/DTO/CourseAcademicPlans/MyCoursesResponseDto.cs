namespace EduAdvisor.Application.DTO.CourseAcademicPlans;

public sealed class MyCoursesResponseDto
{
    public List<MyCourseDto> Completed { get; set; } = [];
    public List<MyCourseDto> InProgress { get; set; } = [];
    public List<MyCourseDto> Remaining { get; set; } = [];
}

public sealed class MyCourseDto
{
    public Guid CourseId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int CreditHours { get; set; }

    public decimal? Grade { get; set; }

    public string Status { get; set; } = string.Empty;
}