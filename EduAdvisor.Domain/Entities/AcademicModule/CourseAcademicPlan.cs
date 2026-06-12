using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Departments;

namespace EduAdvisor.Domain.Entities.AcademicModule;

public  class CourseAcademicPlan : BaseEntity
{
    public Guid CourseId { get; private set; }
    public int Level { get; private set; } 
    public int StandardSemester { get; private set; } 
    public Guid? DepartmentId { get; private set; } 

    public virtual Course Course { get; private set; } = default!;
    public virtual Department? Department { get; private set; }

    private CourseAcademicPlan() { }

    public CourseAcademicPlan(Guid courseId, int level, int standardSemester, Guid? departmentId = null)
    {
        if (courseId == Guid.Empty) throw new ArgumentException("CourseId is required.");
        if (level < 1 || level > 4) throw new ArgumentException("Invalid academic level.");
        if (standardSemester < 1 || standardSemester > 2) throw new ArgumentException("Invalid standard semester.");

        CourseId = courseId;
        Level = level;
        StandardSemester = standardSemester;
        DepartmentId = departmentId;
    }
}