using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Semesters;
using EduAdvisor.Domain.Entities.AcademicModule;
using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Domain.Entities.Enrollments;

public sealed class Enrollment : BaseEntity
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid SemesterId { get; private set; }
    public decimal? Grade { get; private set; }
    public string? RejectionReason { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public Guid? ReviewedByAdvisorId { get; private set; }
    public Guid? GradedByUserId { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public DateTime? GradedAt { get; private set; }

    public Student Student { get; private set; } = default!;
    public Course Course { get; private set; } = default!;
    public Semester Semester { get; private set; } = default!;
    public Advisor? ReviewedByAdvisor { get; private set; }

    private Enrollment() { }

    public Enrollment(Guid studentId, Guid courseId, Guid semesterId)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("StudentId is required.", nameof(studentId));

        if (courseId == Guid.Empty)
            throw new ArgumentException("CourseId is required.", nameof(courseId));

        if (semesterId == Guid.Empty)
            throw new ArgumentException("SemesterId is required.", nameof(semesterId));

        StudentId = studentId;
        CourseId = courseId;
        SemesterId = semesterId;
        Status = EnrollmentStatus.Pending;
    }

    #region Status Methods

    public void Approve(Guid advisorId)
    {
        if (advisorId == Guid.Empty)
            throw new ArgumentException("AdvisorId is required.", nameof(advisorId));

        Status = EnrollmentStatus.Approved;
        ReviewedByAdvisorId = advisorId;
        ReviewedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Reject(Guid advisorId, string reason)
    {
        if (advisorId == Guid.Empty)
            throw new ArgumentException("AdvisorId is required.", nameof(advisorId));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required.", nameof(reason));

        Status = EnrollmentStatus.Rejected;
        RejectionReason = reason.Trim();
        ReviewedByAdvisorId = advisorId;
        ReviewedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void SetGrade(decimal grade, Guid gradedByUserId)
    {
        if (grade < 0 || grade > 4)
            throw new ArgumentException("Grade must be between 0 and 4.");

        if (gradedByUserId == Guid.Empty)
            throw new ArgumentException("GradedByUserId is required.", nameof(gradedByUserId));

        Grade = grade;
        GradedByUserId = gradedByUserId;
        GradedAt = DateTime.UtcNow;
        Status = EnrollmentStatus.Completed;
        UpdateTimestamp();
    }

    #endregion

    #region Helpers

    public bool IsPassed() => Grade.HasValue && Grade.Value >= 2.0m;
    public bool IsPending() => Status == EnrollmentStatus.Pending;
    public bool IsApproved() => Status == EnrollmentStatus.Approved;
    public bool IsRejected() => Status == EnrollmentStatus.Rejected;
    public bool IsCompleted() => Status == EnrollmentStatus.Completed;

    #endregion
}