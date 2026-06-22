using EduAdvisor.Application.Commands.Student;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Domain.Entities.Enrollments;
using EduAdvisor.Domain.Enums.University;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Student;

public sealed class SubmitRegistrationRequestCommandHandler(
    IApplicationDbContext context,
    IGetCurrentUserRepository currentUser)
    : IRequestHandler<SubmitRegistrationRequestCommand, Result<Guid>>
{
    private const decimal MinimumPassingGpa = 2m;
    private const int MinimumCreditHours = 12;
    private const int MaximumCreditHours = 18;
    private const int ProbationMaximumCreditHours = 12;

    public async Task<Result<Guid>> Handle(
        SubmitRegistrationRequestCommand request,
        CancellationToken cancellationToken)
    {
        #region Current Student

        var userId = currentUser.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<Guid>
                .Unauthorized("User is not authenticated.");
        }

        var student = await context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(
                student => student.UserId == userId,
                cancellationToken);

        if (student is null)
        {
            return Result<Guid>
                .NotFound("Student not found.");
        }

        #endregion

        #region Active Semester

        var activeSemester = await context.Semesters
            .AsNoTracking()
            .FirstOrDefaultAsync(
                semester =>
                    semester.IsActive &&
                    semester.IsRegistrationOpen,
                cancellationToken);

        if (activeSemester is null)
        {
            return Result<Guid>.Failure(
                "Registration is currently closed.");
        }

        #endregion

        #region Validate Request

        if (request.SemesterCourseIds is null ||
            request.SemesterCourseIds.Count == 0)
        {
            return Result<Guid>.Failure(
                "At least one course must be selected.");
        }

        var selectedSemesterCourseIds = request.SemesterCourseIds
            .Distinct()
            .ToList();

        if (selectedSemesterCourseIds.Count !=
            request.SemesterCourseIds.Count)
        {
            return Result<Guid>.Failure(
                "Duplicate courses are not allowed.");
        }

        #endregion

        #region Existing Registration Request

        var hasExistingRequest = await context.RegistrationRequests
            .AsNoTracking()
            .AnyAsync(
                registrationRequest =>
                    registrationRequest.StudentId == student.Id &&
                    registrationRequest.SemesterId ==
                    activeSemester.Id &&
                    (registrationRequest.Status ==
                     EnrollmentStatus.Pending ||
                     registrationRequest.Status ==
                     EnrollmentStatus.Approved),
                cancellationToken);

        if (hasExistingRequest)
        {
            return Result<Guid>.Conflict(
                "You already have a registration request for this semester.");
        }

        #endregion

        #region Selected Courses

        var selectedCourses = await context.SemesterCourses
            .AsNoTracking()
            .Where(semesterCourse =>
                semesterCourse.SemesterId == activeSemester.Id &&
                selectedSemesterCourseIds.Contains(semesterCourse.Id))
            .Select(semesterCourse => new SelectedCourseData
            {
                SemesterCourseId = semesterCourse.Id,

                CourseId = semesterCourse
                    .CourseAcademicPlan.CourseId,

                CourseCode = semesterCourse
                    .CourseAcademicPlan.Course.CourseCode,

                CreditHours = semesterCourse
                    .CourseAcademicPlan.Course.CreditHours,

                PrerequisiteCourseIds = semesterCourse
                    .CourseAcademicPlan
                    .Course
                    .Prerequisites
                    .Select(prerequisite =>
                        prerequisite.PrerequisiteCourseId)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        if (selectedCourses.Count !=
            selectedSemesterCourseIds.Count)
        {
            return Result<Guid>.Failure(
                "One or more selected courses are invalid or not offered in the active semester.");
        }

        #endregion

        #region Academic History

        var completedCourses = await context.Enrollments
            .AsNoTracking()
            .Where(enrollment =>
                enrollment.StudentId == student.Id &&
                enrollment.Status == EnrollmentStatus.Completed)
            .Select(enrollment => new
            {
                CourseId = enrollment
                    .SemesterCourse
                    .CourseAcademicPlan
                    .CourseId,

                enrollment.CourseGpa
            })
            .ToListAsync(cancellationToken);

        var passedCourseIds = completedCourses
            .Where(course =>
                course.CourseGpa >= MinimumPassingGpa)
            .Select(course => course.CourseId)
            .ToHashSet();

        #endregion

        #region Passed Courses Validation

        var alreadyPassedCourses = selectedCourses
            .Where(course =>
                passedCourseIds.Contains(course.CourseId))
            .Select(course => course.CourseCode)
            .ToList();

        if (alreadyPassedCourses.Count > 0)
        {
            return Result<Guid>.Failure(
                $"The following courses have already been passed: {string.Join(", ", alreadyPassedCourses)}.");
        }

        #endregion

        #region Prerequisites Validation

        var coursesWithMissingPrerequisites = selectedCourses
            .Where(course =>
                course.PrerequisiteCourseIds.Any(
                    prerequisiteId =>
                        !passedCourseIds.Contains(prerequisiteId)))
            .Select(course => course.CourseCode)
            .ToList();

        if (coursesWithMissingPrerequisites.Count > 0)
        {
            return Result<Guid>.Failure(
                $"Prerequisites have not been completed for: {string.Join(", ", coursesWithMissingPrerequisites)}.");
        }

        #endregion

        #region Credit Hours Validation

        var selectedHours = selectedCourses
            .Sum(course => course.CreditHours);

        if (selectedHours < MinimumCreditHours)
        {
            return Result<Guid>.Failure(
                $"Minimum registered credit hours is {MinimumCreditHours}. Selected hours: {selectedHours}.");
        }

        var maximumAllowedHours =
            student.GPA < MinimumPassingGpa
                ? ProbationMaximumCreditHours
                : MaximumCreditHours;

        if (selectedHours > maximumAllowedHours)
        {
            return Result<Guid>.Failure(
                $"Maximum allowed credit hours is {maximumAllowedHours}. Selected hours: {selectedHours}.");
        }

        #endregion

        #region Create Registration Request

        var registrationRequestId = Guid.NewGuid();

        var registrationRequest =
            new Domain.Entities.AcademicModule.RegistrationRequest(
                registrationRequestId,
                student.Id,
                activeSemester.Id);

        foreach (var selectedCourse in selectedCourses)
        {
            var enrollment = new Enrollment(
                student.Id,
                selectedCourse.SemesterCourseId,
                registrationRequestId);

            registrationRequest.AddEnrollment(enrollment);
        }

        context.RegistrationRequests.Add(registrationRequest);

        await context.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<Guid>.Success(
            registrationRequestId,
            "Registration request submitted successfully.");
    }

    private sealed class SelectedCourseData
    {
        public Guid SemesterCourseId { get; init; }

        public Guid CourseId { get; init; }

        public string CourseCode { get; init; } = string.Empty;

        public int CreditHours { get; init; }

        public List<Guid> PrerequisiteCourseIds { get; init; } = [];
    }
}