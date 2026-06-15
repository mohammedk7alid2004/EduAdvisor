using EduAdvisor.Application.Commands.Student;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Domain.Entities.AcademicModule;
using EduAdvisor.Domain.Entities.Enrollments;
using EduAdvisor.Domain.Enums.University;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Student;

public sealed class SubmitRegistrationRequestCommandHandler(
    IApplicationDbContext context,
    IGetCurrentUserRepository currentUser)
    : IRequestHandler<SubmitRegistrationRequestCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        SubmitRegistrationRequestCommand request,
        CancellationToken cancellationToken)
    {
        #region Current Student

        var userId = currentUser.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Result<Guid>.Unauthorized("User is not authenticated.");

        var student = await context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);

        if (student is null)
            return Result<Guid>.NotFound("Student not found.");

        #endregion

        #region Active Semester

        var activeSemester = await context.Semesters
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.IsActive && x.IsRegistrationOpen,
                cancellationToken);

        if (activeSemester is null)
            return Result<Guid>.Failure(
                "Registration is currently closed.");

        #endregion

        #region Selected Courses

        var selectedCourses = await context.SemesterCourses
            .Where(x =>
                x.SemesterId == activeSemester.Id &&
                request.SemesterCourseIds.Contains(x.Id))
            .Select(x => new
            {
                SemesterCourseId = x.Id,
                CourseId = x.CourseAcademicPlan.CourseId,
                CreditHours = x.CourseAcademicPlan.Course.CreditHours
            })
            .ToListAsync(cancellationToken);

        if (selectedCourses.Count != request.SemesterCourseIds.Count)
        {
            return Result<Guid>.Failure(
                "One or more selected courses are invalid.");
        }

        #endregion

        #region Existing Registration Request

        var existingRequest = await context.RegistrationRequests
            .AnyAsync(x =>
                x.StudentId == student.Id &&
                x.SemesterId == activeSemester.Id &&
                (x.Status == EnrollmentStatus.Pending ||
                 x.Status == EnrollmentStatus.Approved),
                cancellationToken);

        if (existingRequest)
        {
            return Result<Guid>.Conflict(
                "You already have a registration request for this semester.");
        }

        #endregion

        #region Failed Courses

        var hasFailedCourses = await context.Enrollments
            .AnyAsync(x =>
                x.StudentId == student.Id &&
                x.Status == EnrollmentStatus.Completed &&
                x.CourseGpa < 1,
                cancellationToken);

        #endregion

        #region Credit Hours Validation

        var selectedHours = selectedCourses.Sum(x => x.CreditHours);

        const int minimumHours = 12;

        if (selectedHours < minimumHours)
        {
            return Result<Guid>.Failure(
                $"Minimum registered credit hours is {minimumHours}.");
        }

        var maxAllowedHours =
            student.GetMaxAllowedCreditHours(hasFailedCourses);

        if (selectedHours > maxAllowedHours)
        {
            return Result<Guid>.Failure(
                $"Maximum allowed credit hours is {maxAllowedHours}. Selected hours: {selectedHours}.");
        }

        #endregion

        #region Passed Courses Validation

        var passedCourseIds = await context.Enrollments
            .Where(x =>
                x.StudentId == student.Id &&
                x.Status == EnrollmentStatus.Completed &&
                x.CourseGpa >= 1)
            .Select(x => x.SemesterCourse.CourseAcademicPlan.CourseId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (selectedCourses.Any(x =>
            passedCourseIds.Contains(x.CourseId)))
        {
            return Result<Guid>.Failure(
                "One or more selected courses have already been passed.");
        }

        #endregion

        #region Create Registration Request

        var registrationRequestId = Guid.NewGuid();

        var registrationRequest = new Domain.Entities.AcademicModule.RegistrationRequest(
            registrationRequestId,
            student.Id,
            activeSemester.Id);

        foreach (var course in selectedCourses)
        {
            registrationRequest.AddEnrollment(
                new Enrollment(
                    student.Id,
                    course.SemesterCourseId,
                    registrationRequestId));
        }

        context.RegistrationRequests.Add(registrationRequest);

        await context.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<Guid>.Success(
            registrationRequestId,
            "Registration request submitted successfully.");
    }
}