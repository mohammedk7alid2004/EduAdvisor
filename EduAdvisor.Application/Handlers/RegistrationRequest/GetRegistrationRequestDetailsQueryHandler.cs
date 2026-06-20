using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.RegistrationRequest;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.RegistrationRequests;
using EduAdvisor.Domain.Enums.University;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.RegistrationRequest;

public sealed class GetRegistrationRequestDetailsQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<
        GetRegistrationRequestDetailsQuery,
        Result<RegistrationRequestDetailsDto>>
{
    public async Task<Result<RegistrationRequestDetailsDto>> Handle(
        GetRegistrationRequestDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var registrationRequest = await context.RegistrationRequests
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Student)
                .ThenInclude(x => x.User)
            .Include(x => x.Student)
                .ThenInclude(x => x.Department)
            .Include(x => x.Enrollments)
                .ThenInclude(x => x.SemesterCourse)
                .ThenInclude(x => x.CourseAcademicPlan)
                .ThenInclude(x => x.Course)
                .ThenInclude(x => x.Prerequisites)
                .ThenInclude(x => x.PrerequisiteCourse)
            .FirstOrDefaultAsync(
                x => x.Id == request.RegistrationRequestId,
                cancellationToken);

        if (registrationRequest is null)
        {
            return Result<RegistrationRequestDetailsDto>.NotFound(
                "Registration request not found.");
        }

        var completedCourses = await context.Enrollments
            .AsNoTracking()
            .Where(x =>
                x.StudentId == registrationRequest.StudentId &&
                x.Status == EnrollmentStatus.Completed)
            .Select(x => new
            {
                x.CourseGpa,
                CourseId = x.SemesterCourse.CourseAcademicPlan.CourseId
            })
            .ToListAsync(cancellationToken);

        var passedCourseIds = completedCourses
            .Where(x => x.CourseGpa >= 1)
            .Select(x => x.CourseId)
            .ToHashSet();

        var failedCourseIds = completedCourses
            .Where(x => x.CourseGpa < 1)
            .Select(x => x.CourseId)
            .Where(courseId => !passedCourseIds.Contains(courseId))
            .ToHashSet();

        var courses = registrationRequest.Enrollments
            .Select(enrollment =>
            {
                var course = enrollment.SemesterCourse
                    .CourseAcademicPlan
                    .Course;

                var missingPrerequisites = course.Prerequisites
                    .Where(prerequisite =>
                        !passedCourseIds.Contains(
                            prerequisite.PrerequisiteCourseId))
                    .Select(prerequisite =>
                        prerequisite.PrerequisiteCourse.CourseName)
                    .Distinct()
                    .ToList();

                return new RequestedCourseDto
                {
                    EnrollmentId = enrollment.Id,
                    CourseId = course.Id,
                    CourseCode = course.CourseCode,
                    CourseName = course.CourseName,
                    CreditHours = course.CreditHours,
                    IsRetake = failedCourseIds.Contains(course.Id),
                    HasMissingPrerequisites = missingPrerequisites.Count > 0,
                    MissingPrerequisites = missingPrerequisites
                };
            })
            .ToList();

        var student = registrationRequest.Student;

        var response = new RegistrationRequestDetailsDto
        {
            RegistrationRequestId = registrationRequest.Id,
            StudentName = student.User.FullName.Trim(),
            StudentCode = student.StudentCode,
            StudentPhotoUrl = student.User.ProfileImageUrl,
            DepartmentName = student.Department.Name,
            AcademicYear = student.AcademicYear,
            GPA = student.GPA,
            CompletedHours = student.CompletedHours,
            FailedCoursesCount = failedCourseIds.Count,
            Status = registrationRequest.Status,
            Courses = courses
        };

        return Result<RegistrationRequestDetailsDto>.Success(response);
    }
}