using EduAdvisor.Application.DTO.CourseAcademicPlans;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.CourseAcademicPlans;
using EduAdvisor.Domain.Enums.University;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AcademicModules;

public sealed class GetMyCoursesQueryHandler(
    IApplicationDbContext context,
    IGetCurrentUserRepository currentUserRepository,
    IStringLocalizer<GetMyCoursesQueryHandler> localizer)
    : IRequestHandler<GetMyCoursesQuery, Result<MyCoursesResponseDto>>
{
    public async Task<Result<MyCoursesResponseDto>> Handle(
        GetMyCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = currentUserRepository.GetUserId();

        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return Result<MyCoursesResponseDto>.Failure(
                localizer["Unauthorized"],
                401);
        }

        var student = await context.Students
            .AsNoTracking()
            .Where(student => student.UserId == currentUserId)
            .Select(student => new
            {
                student.Id,
                student.DepartmentId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (student is null)
        {
            return Result<MyCoursesResponseDto>.Failure(
                localizer["StudentNotFound"],
                404);
        }

        var enrollments = await context.Enrollments
            .AsNoTracking()
            .Where(enrollment => enrollment.StudentId == student.Id)
            .Select(enrollment => new
            {
                enrollment.SemesterCourse.CourseAcademicPlan.CourseId,
                enrollment.Status,
                enrollment.CourseGpa,
                CourseCode = enrollment.SemesterCourse.CourseAcademicPlan.Course.CourseCode,
                CourseName = enrollment.SemesterCourse.CourseAcademicPlan.Course.CourseName,
                enrollment.SemesterCourse.CourseAcademicPlan.Course.CreditHours
            })
            .ToListAsync(cancellationToken);

        var completedCourses = enrollments
            .Where(enrollment => enrollment.Status == EnrollmentStatus.Approved)
            .DistinctBy(enrollment => enrollment.CourseId)
            .ToList();

        var inProgressCourses = enrollments
            .Where(enrollment => enrollment.Status == EnrollmentStatus.Pending)
            .DistinctBy(enrollment => enrollment.CourseId)
            .ToList();

        var completedCourseIds = completedCourses
            .Select(enrollment => enrollment.CourseId)
            .ToHashSet();

        var inProgressCourseIds = inProgressCourses
            .Select(enrollment => enrollment.CourseId)
            .ToHashSet();

        var academicPlanCourses = await context.CourseAcademicPlans
            .AsNoTracking()
            .Where(plan =>
                plan.DepartmentId == null ||
                plan.DepartmentId == student.DepartmentId)
            .Select(plan => new
            {
                plan.CourseId,
                CourseCode = plan.Course.CourseCode,
                CourseName = plan.Course.CourseName,
                plan.Course.CreditHours
            })
            .ToListAsync(cancellationToken);

        var response = new MyCoursesResponseDto
        {
            Completed = completedCourses
                .Select(enrollment => new MyCourseDto
                {
                    CourseId = enrollment.CourseId,
                    Code = enrollment.CourseCode,
                    Name = enrollment.CourseName,
                    CreditHours = enrollment.CreditHours,
                    Grade = enrollment.CourseGpa,
                    Status = "Completed"
                })
                .ToList(),

            InProgress = inProgressCourses
                .Select(enrollment => new MyCourseDto
                {
                    CourseId = enrollment.CourseId,
                    Code = enrollment.CourseCode,
                    Name = enrollment.CourseName,
                    CreditHours = enrollment.CreditHours,
                    Grade = enrollment.CourseGpa,
                    Status = "InProgress"
                })
                .ToList(),

            Remaining = academicPlanCourses
                .Where(plan =>
                    !completedCourseIds.Contains(plan.CourseId) &&
                    !inProgressCourseIds.Contains(plan.CourseId))
                .Select(plan => new MyCourseDto
                {
                    CourseId = plan.CourseId,
                    Code = plan.CourseCode,
                    Name = plan.CourseName,
                    CreditHours = plan.CreditHours,
                    Grade = null,
                    Status = "Remaining"
                })
                .ToList()
        };

        return Result<MyCoursesResponseDto>.Success(
            response,
            localizer["CoursesLoadedSuccessfully"]);
    }
}