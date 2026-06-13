using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Semesters;

namespace EduAdvisor.Domain.Entities.AcademicModule;

public  class SemesterCourse : BaseEntity
{
    public Guid SemesterId { get; private set; } // The active physical semester (e.g., Fall 2026)
    public Guid CourseAcademicPlanId { get; private set; } // Links to the plan (which contains Course, Level, Dept)

    public virtual Semester Semester { get; private set; } = default!;
    public virtual CourseAcademicPlan CourseAcademicPlan { get; private set; } = default!;

    private SemesterCourse() { }

    public SemesterCourse(Guid semesterId, Guid courseAcademicPlanId)
    {
        if (semesterId == Guid.Empty) throw new ArgumentException("SemesterId is required.");
        if (courseAcademicPlanId == Guid.Empty) throw new ArgumentException("CourseAcademicPlanId is required.");

        SemesterId = semesterId;
        CourseAcademicPlanId = courseAcademicPlanId;
    }
    public void Update(Guid semesterId, Guid courseAcademicPlanId)
    {
        if (semesterId == Guid.Empty) throw new ArgumentException("SemesterId is required.");
        if (courseAcademicPlanId == Guid.Empty) throw new ArgumentException("CourseAcademicPlanId is required.");

        SemesterId = semesterId;
        CourseAcademicPlanId = courseAcademicPlanId;
        UpdateTimestamp();
    }
}