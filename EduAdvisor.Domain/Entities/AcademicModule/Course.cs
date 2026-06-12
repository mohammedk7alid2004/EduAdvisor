using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Departments;
using EduAdvisor.Domain.Enums;
using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Domain.Entities.AcademicModule;

public class Course : BaseEntity
{
    public string CourseCode { get; private set; } = string.Empty;
    public string CourseName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int CreditHours { get; private set; }
    public CourseType Type { get; private set; }

    public int StandardLevel { get; private set; }
    public int StandardSemester { get; private set; }

    public Guid? DepartmentId { get; private set; }
    public virtual Department? Department { get; private set; }

    public virtual ICollection<CoursePrerequisite> Prerequisites { get; private set; } = new List<CoursePrerequisite>();

    protected Course() { }

    public Course(
        string courseCode,
        string courseName,
        string? description,
        int creditHours,
        CourseType type,
        int standardLevel,
        int standardSemester,
        Guid? departmentId = null)
    {
        CourseCode = courseCode;
        CourseName = courseName;
        Description = description;
        CreditHours = creditHours;
        Type = type;
        StandardLevel = standardLevel;
        StandardSemester = standardSemester;
        DepartmentId = departmentId;
    }

    public void UpdateDetails(string courseName, string? description, int creditHours, CourseType type, int standardLevel, int standardSemester, Guid? departmentId)
    {
        CourseName = courseName;
        Description = description;
        CreditHours = creditHours;
        Type = type;
        StandardLevel = standardLevel;
        StandardSemester = standardSemester;
        DepartmentId = departmentId;
        UpdateTimestamp();
    }
}