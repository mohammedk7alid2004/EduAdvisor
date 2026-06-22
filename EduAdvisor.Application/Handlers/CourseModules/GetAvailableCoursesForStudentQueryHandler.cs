using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.CourseModules;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.CourseModules;
using EduAdvisor.Domain.Enums.University;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseModules;

public sealed class GetAvailableCoursesForStudentQueryHandler(
    IApplicationDbContext context,
    IGetCurrentUserRepository currentUser)
    : IRequestHandler<
        GetAvailableCoursesForStudentQuery,
        Result<List<AvailableCourseDto>>>
{
    private const decimal MinimumPassingGpa = 2m;

    public async Task<Result<List<AvailableCourseDto>>> Handle(
        GetAvailableCoursesForStudentQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<List<AvailableCourseDto>>
                .Unauthorized("User is not authenticated.");
        }

        var studentId = await context.Students
            .AsNoTracking()
            .Where(student => student.UserId == userId)
            .Select(student => student.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentId == Guid.Empty)
        {
            return Result<List<AvailableCourseDto>>
                .NotFound("Student not found.");
        }

        var activeSemesterId = await context.Semesters
            .AsNoTracking()
            .Where(semester =>
                semester.IsActive &&
                semester.IsRegistrationOpen)
            .Select(semester => semester.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeSemesterId == Guid.Empty)
        {
            return Result<List<AvailableCourseDto>>
                .NotFound("No active registration semester found.");
        }

        var completedCourses = await context.Enrollments
            .AsNoTracking()
            .Where(enrollment =>
                enrollment.StudentId == studentId &&
                enrollment.Status == EnrollmentStatus.Completed)
            .Select(enrollment => new
            {
                CourseId = enrollment.SemesterCourse
                    .CourseAcademicPlan.CourseId,

                enrollment.CourseGpa
            })
            .ToListAsync(cancellationToken);

        var passedCourseIds = completedCourses
            .Where(course => course.CourseGpa >= MinimumPassingGpa)
            .Select(course => course.CourseId)
            .ToHashSet();

        var failedCourseIds = completedCourses
            .Where(course =>
                course.CourseGpa < MinimumPassingGpa &&
                !passedCourseIds.Contains(course.CourseId))
            .Select(course => course.CourseId)
            .ToHashSet();

        var registeredCourseIds = await context.Enrollments
            .AsNoTracking()
            .Where(enrollment =>
                enrollment.StudentId == studentId &&
                enrollment.SemesterCourse.SemesterId == activeSemesterId &&
                (enrollment.Status == EnrollmentStatus.Pending ||
                 enrollment.Status == EnrollmentStatus.Approved))
            .Select(enrollment => enrollment.SemesterCourse
                .CourseAcademicPlan.CourseId)
            .Distinct()
            .ToHashSetAsync(cancellationToken);

        var candidateCourses = await context.SemesterCourses
            .AsNoTracking()
            .Where(semesterCourse =>
                semesterCourse.SemesterId == activeSemesterId)
            .Select(semesterCourse => new CourseCandidateDto
            {
                SemesterCourseId = semesterCourse.Id,

                CourseId = semesterCourse.CourseAcademicPlan.CourseId,

                CourseCode = semesterCourse.CourseAcademicPlan
                    .Course.CourseCode,

                CourseName = semesterCourse.CourseAcademicPlan
                    .Course.CourseName,

                CreditHours = semesterCourse.CourseAcademicPlan
                    .Course.CreditHours,

                PrerequisiteIds = semesterCourse.CourseAcademicPlan
                    .Course.Prerequisites
                    .Select(prerequisite =>
                        prerequisite.PrerequisiteCourseId)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var availableCourses = candidateCourses
            .Where(course =>
                !passedCourseIds.Contains(course.CourseId) &&
                !registeredCourseIds.Contains(course.CourseId) &&
                course.PrerequisiteIds.All(passedCourseIds.Contains))
            .Select(course => new AvailableCourseDto
            {
                SemesterCourseId = course.SemesterCourseId,
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                CreditHours = course.CreditHours,
                IsRetake = failedCourseIds.Contains(course.CourseId)
            })
            .OrderByDescending(course => course.IsRetake)
            .ThenBy(course => course.CourseCode)
            .ToList();

        return Result<List<AvailableCourseDto>>
            .Success(availableCourses);
    }
}