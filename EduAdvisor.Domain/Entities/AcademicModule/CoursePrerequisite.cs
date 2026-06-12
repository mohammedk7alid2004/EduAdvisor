namespace EduAdvisor.Domain.Entities.AcademicModule;

public class CoursePrerequisite
{
    // المادة الأساسية التي يريد الطالب تسجيلها (مثال: CS412 - Advanced DB)
    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    // المادة التي يجب أن يكون الطالب قد نجح فيها أولاً (مثال: CS211 - Database 1)
    public Guid PrerequisiteCourseId { get; set; }
    public virtual Course PrerequisiteCourse { get; set; } = null!;
}