using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Departments;

namespace EduAdvisor.Domain.Entities.AuthModule;

public class Student : BaseEntity
{
    public string UserId { get; private set; } = string.Empty;
    public string StudentCode { get; private set; } = string.Empty;
    public int AcademicYear { get; private set; }
    public int CompletedHours { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid? AdvisorId { get; private set; }
    public decimal GPA { get; private set; }

    public User User { get; private set; } = default!;
    public Department Department { get; private set; } = default!;
    public Advisor? Advisor { get; private set; }

    private Student() { }

    public Student(string userId, string studentCode, Guid departmentId, int academicYear = 1)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required.", nameof(userId));

        if (string.IsNullOrWhiteSpace(studentCode))
            throw new ArgumentException("StudentCode is required.", nameof(studentCode));

        if (departmentId == Guid.Empty)
            throw new ArgumentException("DepartmentId is required.", nameof(departmentId));

        if (academicYear <= 0)
            throw new ArgumentOutOfRangeException(nameof(academicYear), "Academic year must be greater than zero.");

        UserId = userId;
        StudentCode = studentCode.Trim();
        DepartmentId = departmentId;
        AcademicYear = academicYear;
        CompletedHours = 0;
        GPA = 0;
    }

    #region Updates

    public void UpdateGPA(decimal gpa)
    {
        if (gpa < 0 || gpa > 4)
            throw new ArgumentException("GPA must be between 0 and 4.");

        GPA = gpa;
        UpdateTimestamp();
    }

    public void UpdateCompletedHours(int hours)
    {
        if (hours < 0)
            throw new ArgumentOutOfRangeException(nameof(hours), "Completed hours cannot be negative.");

        CompletedHours = hours;
        UpdateTimestamp();
    }

    public void UpdateAcademicYear(int year)
    {
        if (year <= 0)
            throw new ArgumentOutOfRangeException(nameof(year), "Academic year must be greater than zero.");

        AcademicYear = year;
        UpdateTimestamp();
    }

    public void AssignAdvisor(Guid advisorId)
    {
        if (advisorId == Guid.Empty)
            throw new ArgumentException("AdvisorId is required.", nameof(advisorId));

        AdvisorId = advisorId;
        UpdateTimestamp();
    }

    public void RemoveAdvisor()
    {
        AdvisorId = null;
        UpdateTimestamp();
    }

    #endregion
}