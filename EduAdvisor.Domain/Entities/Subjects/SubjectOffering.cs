using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Departments;
using EduAdvisor.Domain.Entities.Semesters;

namespace EduAdvisor.Domain.Entities.Subjects;

public sealed class SubjectOffering : BaseEntity
{
    public Guid SubjectId { get; private set; }
    public Guid SemesterId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public int Level { get; private set; }
    public int MaxCapacity { get; private set; }
    public int CurrentEnrollment { get; private set; }

    public Subject Subject { get; private set; } = default!;
    public Semester Semester { get; private set; } = default!;
    public Department Department { get; private set; } = default!;

    private SubjectOffering() { }

    public SubjectOffering(
        Guid subjectId,
        Guid semesterId,
        Guid departmentId,
        int level,
        int maxCapacity)
    {
        if (subjectId == Guid.Empty)
            throw new ArgumentException("SubjectId is required.", nameof(subjectId));

        if (semesterId == Guid.Empty)
            throw new ArgumentException("SemesterId is required.", nameof(semesterId));

        if (departmentId == Guid.Empty)
            throw new ArgumentException("DepartmentId is required.", nameof(departmentId));

        if (level <= 0)
            throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");

        if (maxCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Max capacity must be greater than zero.");

        SubjectId = subjectId;
        SemesterId = semesterId;
        DepartmentId = departmentId;
        Level = level;
        MaxCapacity = maxCapacity;
        CurrentEnrollment = 0;
    }

    #region Capacity

    public bool HasAvailableSeats()
        => CurrentEnrollment < MaxCapacity;

    public void IncrementEnrollment()
    {
        if (!HasAvailableSeats())
            throw new InvalidOperationException("No available seats.");

        CurrentEnrollment++;
        UpdateTimestamp();
    }

    public void DecrementEnrollment()
    {
        if (CurrentEnrollment > 0)
            CurrentEnrollment--;

        UpdateTimestamp();
    }

    #endregion

    #region Updates

    public void UpdateLevel(int level)
    {
        if (level <= 0)
            throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");

        Level = level;
        UpdateTimestamp();
    }

    public void UpdateMaxCapacity(int maxCapacity)
    {
        if (maxCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Max capacity must be greater than zero.");

        if (maxCapacity < CurrentEnrollment)
            throw new InvalidOperationException(
                $"Max capacity ({maxCapacity}) cannot be less than current enrollment ({CurrentEnrollment}).");

        MaxCapacity = maxCapacity;
        UpdateTimestamp();
    }

    #endregion
}