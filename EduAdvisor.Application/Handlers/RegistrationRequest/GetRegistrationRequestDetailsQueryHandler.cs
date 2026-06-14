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
            .Include(x => x.Student)
                .ThenInclude(x => x.User)
            .Include(x => x.Student)
                .ThenInclude(x => x.Department)
            .Include(x => x.Enrollments)
                .ThenInclude(x => x.SemesterCourse)
                    .ThenInclude(x => x.CourseAcademicPlan)
                        .ThenInclude(x => x.Course)
                            .ThenInclude(x => x.Prerequisites)
            .FirstOrDefaultAsync(
                x => x.Id == request.RegistrationRequestId,
                cancellationToken);

        if (registrationRequest is null)
        {
            return Result<RegistrationRequestDetailsDto>
                .NotFound("Registration request not found.");
        }

        var passedCourseIds = await context.Enrollments
            .AsNoTracking()
            .Where(x =>
                x.StudentId == registrationRequest.StudentId &&
                x.Status == EnrollmentStatus.Completed &&
                x.CourseGpa >= 1)
            .Select(x => x.SemesterCourse.CourseAcademicPlan.CourseId)
            .Distinct()
            .ToHashSetAsync(cancellationToken);

        var failedCourseIds = await context.Enrollments
            .AsNoTracking()
            .Where(x =>
                x.StudentId == registrationRequest.StudentId &&
                x.Status == EnrollmentStatus.Completed &&
                x.CourseGpa < 1)
            .Select(x => x.SemesterCourse.CourseAcademicPlan.CourseId)
            .Distinct()
            .ToHashSetAsync(cancellationToken);

        var dto = new RegistrationRequestDetailsDto
        {
            RegistrationRequestId = registrationRequest.Id,

            StudentName =
                $"{registrationRequest.Student.User.FullName} ",

            StudentCode =
                registrationRequest.Student.StudentCode,

            DepartmentName =
                registrationRequest.Student.Department.Name,
            StudentPhotoUrl =
                registrationRequest.Student.User.ProfileImageUrl,
            AcademicYear =
                registrationRequest.Student.AcademicYear,

            GPA =
                registrationRequest.Student.GPA,

            CompletedHours =
                registrationRequest.Student.CompletedHours,

            FailedCoursesCount =
                failedCourseIds.Count,

            Status =
                registrationRequest.Status,


            Courses = registrationRequest.Enrollments
                .Select(enrollment =>
                {
                    var course =
                        enrollment.SemesterCourse
                            .CourseAcademicPlan
                            .Course;

                    var missingPrerequisites =
                        course.Prerequisites
                            .Where(p =>
                                !passedCourseIds.Contains(
                                    p.PrerequisiteCourseId))
                            .Select(p =>
                                p.PrerequisiteCourse.CourseName)
                            .ToList();

                    return new RequestedCourseDto
                    {
                        EnrollmentId = enrollment.Id,

                        CourseId = course.Id,

                        CourseCode = course.CourseCode,

                        CourseName = course.CourseName,

                        CreditHours = course.CreditHours,

                        IsRetake =
                            failedCourseIds.Contains(course.Id),

                        HasMissingPrerequisites =
                            missingPrerequisites.Any(),

                        MissingPrerequisites =
                            missingPrerequisites
                    };
                })
                .ToList()
        };

        return Result<RegistrationRequestDetailsDto>
            .Success(dto);
    }
}