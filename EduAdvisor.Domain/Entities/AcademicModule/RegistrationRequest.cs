using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Domain.Entities.Semesters;
using EduAdvisor.Domain.Entities.Enrollments;
using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Domain.Entities.AcademicModule;

public sealed class RegistrationRequest : BaseEntity
{
    private readonly List<Enrollment> _enrollments = new();

    public Guid StudentId { get; private set; }
    public Guid SemesterId { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public Guid? ReviewedByAdvisorId { get; private set; } // تم جعلها private set للحفاظ على الـ Encapsulation

    public Student Student { get; private set; } = default!;
    public Semester Semester { get; private set; } = default!;

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private RegistrationRequest() { }

    public RegistrationRequest(Guid studentId, Guid semesterId, string? notes = null)
    {
        if (studentId == Guid.Empty) throw new ArgumentException("StudentId is required.");
        if (semesterId == Guid.Empty) throw new ArgumentException("SemesterId is required.");

        StudentId = studentId;
        SemesterId = semesterId;
        Notes = notes?.Trim();
        Status = EnrollmentStatus.Pending;
        SubmittedAt = DateTime.UtcNow;
    }

    public void AddEnrollment(Enrollment enrollment)
    {
        if (enrollment == null) throw new ArgumentNullException(nameof(enrollment));
        _enrollments.Add(enrollment);
    }

    // تعديل الـ Approve لاستقبال الـ AdvisorId صراحة وتوزيعه على المواد
    public void Approve(Guid advisorId)
    {
        if (advisorId == Guid.Empty) throw new ArgumentException("AdvisorId is required to approve the request.");

        Status = EnrollmentStatus.Approved;
        ReviewedByAdvisorId = advisorId;

        foreach (var enrollment in _enrollments)
        {
            enrollment.Approve(advisorId);
        }
        UpdateTimestamp();
    }

    public void Reject(Guid advisorId, string reason)
    {
        if (advisorId == Guid.Empty) throw new ArgumentException("AdvisorId is required to reject the request.");
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Rejection reason is required.");

        Status = EnrollmentStatus.Rejected;
        ReviewedByAdvisorId = advisorId;
        Notes = $"[Rejection Reason]: {reason.Trim()} | {Notes}";

        foreach (var enrollment in _enrollments)
        {
            enrollment.Reject(advisorId, reason);
        }
        UpdateTimestamp();
    }
}