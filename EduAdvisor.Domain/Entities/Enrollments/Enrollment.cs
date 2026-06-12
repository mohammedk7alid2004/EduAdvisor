using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.AcademicModule;
using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Domain.Entities.Enrollments;

public sealed class Enrollment : BaseEntity
{
    public Guid StudentId { get; private set; }
    public Guid SemesterCourseId { get; private set; }

    public Guid RegistrationRequestId { get; private set; }

    public decimal? CoursePercentage { get; private set; }
    public decimal? CourseGpa { get; private set; } 
    public string? RejectionReason { get; private set; }
    public EnrollmentStatus Status { get; private set; } 
    public Guid? ReviewedByAdvisorId { get; private set; }
    public Guid? GradedByUserId { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public DateTime? GradedAt { get; private set; }

    public Student Student { get; private set; } = default!;
    public SemesterCourse SemesterCourse { get; private set; } = default!;
    public RegistrationRequest RegistrationRequest { get; private set; } = default!;
    public Advisor? ReviewedByAdvisor { get; private set; }

    private Enrollment() { }

    public Enrollment(Guid studentId, Guid semesterCourseId, Guid registrationRequestId)
    {
        if (studentId == Guid.Empty) throw new ArgumentException("StudentId is required.");
        if (semesterCourseId == Guid.Empty) throw new ArgumentException("SemesterCourseId is required.");
        if (registrationRequestId == Guid.Empty) throw new ArgumentException("RegistrationRequestId is required.");

        StudentId = studentId;
        SemesterCourseId = semesterCourseId;
        RegistrationRequestId = registrationRequestId;
        Status = EnrollmentStatus.Pending;
    }

    public void Approve(Guid advisorId)
    {
        if (advisorId == Guid.Empty) throw new ArgumentException("AdvisorId is required.");
        Status = EnrollmentStatus.Approved;
        ReviewedByAdvisorId = advisorId;
        ReviewedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Reject(Guid advisorId, string reason)
    {
        if (advisorId == Guid.Empty) throw new ArgumentException("AdvisorId is required.");
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason is required.");
        Status = EnrollmentStatus.Rejected;
        RejectionReason = reason.Trim();
        ReviewedByAdvisorId = advisorId;
        ReviewedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void SetFinalResult(decimal percentage, decimal gpa, Guid gradedByUserId)
    {
        if (percentage < 0 || percentage > 100) throw new ArgumentException("Percentage must be between 0 and 100.");
        if (gpa < 0 || gpa > 4) throw new ArgumentException("GPA must be between 0 and 4.");
        if (gradedByUserId == Guid.Empty) throw new ArgumentException("GradedByUserId is required.");

        CoursePercentage = percentage;
        CourseGpa = gpa;
        GradedByUserId = gradedByUserId;
        GradedAt = DateTime.UtcNow;
        Status = EnrollmentStatus.Completed;
        UpdateTimestamp();
    }

    public bool IsPassed() => CourseGpa.HasValue && CourseGpa.Value >= 1.0m;
}