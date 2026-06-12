namespace EduAdvisor.Domain.Entities.AcademicModule;

public class CoursePrerequisite
{
    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public Guid PrerequisiteCourseId { get; set; }
    public virtual Course PrerequisiteCourse { get; set; } = null!;
}