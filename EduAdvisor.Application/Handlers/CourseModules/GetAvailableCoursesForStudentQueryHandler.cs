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
    : IRequestHandler<GetAvailableCoursesForStudentQuery, Result<List<AvailableCourseDto>>>
{
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
            .Where(x => x.UserId == userId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentId == Guid.Empty)
        {
            return Result<List<AvailableCourseDto>>
                .NotFound("Student not found.");
        }

        var activeSemesterId = await context.Semesters
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsRegistrationOpen)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeSemesterId == Guid.Empty)
        {
            return Result<List<AvailableCourseDto>>
                .NotFound("No active semester found.");
        }

        var passedCourseIds = await context.Enrollments
            .AsNoTracking()
            .Where(x =>
                x.StudentId == studentId &&
                x.Status == EnrollmentStatus.Completed &&
                x.CourseGpa >= 1)
            .Select(x => x.SemesterCourse.CourseAcademicPlan.CourseId)
            .Distinct()
            .ToHashSetAsync(cancellationToken);

        var failedCourseIds = await context.Enrollments
            .AsNoTracking()
            .Where(x =>
                x.StudentId == studentId &&
                x.Status == EnrollmentStatus.Completed &&
                x.CourseGpa < 1)
            .Select(x => x.SemesterCourse.CourseAcademicPlan.CourseId)
            .Distinct()
            .ToHashSetAsync(cancellationToken);

        var alreadyRegisteredCourseIds = await context.Enrollments
            .AsNoTracking()
            .Where(x =>
                x.StudentId == studentId &&
                (x.Status == EnrollmentStatus.Pending ||
                 x.Status == EnrollmentStatus.Approved) &&
                x.SemesterCourse.SemesterId == activeSemesterId)
            .Select(x => x.SemesterCourse.CourseAcademicPlan.CourseId)
            .Distinct()
            .ToHashSetAsync(cancellationToken);

        var candidateCourses = await context.SemesterCourses
            .AsNoTracking()
            .Where(x => x.SemesterId == activeSemesterId)
            .Select(x => new CourseCandidateDto
            {
                SemesterCourseId = x.Id,
                CourseId = x.CourseAcademicPlan.CourseId,
                CourseCode = x.CourseAcademicPlan.Course.CourseCode,
                CourseName = x.CourseAcademicPlan.Course.CourseName,
                CreditHours = x.CourseAcademicPlan.Course.CreditHours,

                PrerequisiteIds = x.CourseAcademicPlan.Course.Prerequisites
                    .Select(p => p.PrerequisiteCourseId)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var availableCourses = candidateCourses
            .Where(course =>
                !passedCourseIds.Contains(course.CourseId)
                && !alreadyRegisteredCourseIds.Contains(course.CourseId)
                && course.PrerequisiteIds.All(passedCourseIds.Contains))
            .Select(course => new AvailableCourseDto
            {
                SemesterCourseId = course.SemesterCourseId,
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                CreditHours = course.CreditHours,
                IsRetake = failedCourseIds.Contains(course.CourseId)
            })
            .OrderBy(x => x.CourseCode)
            .ToList();

        return Result<List<AvailableCourseDto>>
            .Success(availableCourses);
    }
}