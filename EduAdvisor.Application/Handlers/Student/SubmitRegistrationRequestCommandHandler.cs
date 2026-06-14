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
        var userId = currentUser.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Result<Guid>.Unauthorized("User is not authenticated.");

        var student = await context.Students
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);

        if (student is null)
            return Result<Guid>.NotFound("Student not found.");

        var activeSemester = await context.Semesters
            .FirstOrDefaultAsync(
                x => x.IsActive && x.IsRegistrationOpen,
                cancellationToken);

        if (activeSemester is null)
            return Result<Guid>.NotFound("No active semester found.");

        var semesterCourses = await context.SemesterCourses
            .Where(x =>
                x.SemesterId == activeSemester.Id &&
                request.SemesterCourseIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (semesterCourses.Count != request.SemesterCourseIds.Count)
            return Result<Guid>.Failure("One or more courses are invalid.");

        var existingPendingRequest = await context.Enrollments
            .AnyAsync(x =>
                x.StudentId == student.Id &&
                (x.Status == EnrollmentStatus.Pending ||
                 x.Status == EnrollmentStatus.Approved) &&
                x.SemesterCourse.SemesterId == activeSemester.Id,
                cancellationToken);

        if (existingPendingRequest)
            return Result<Guid>.Conflict(
                "You already have a registration request for this semester.");

        var registrationRequestId = Guid.NewGuid();

        var registrationRequest = new RegistrationRequest(
            registrationRequestId,
            student.Id,
            activeSemester.Id);

        foreach (var semesterCourseId in request.SemesterCourseIds)
        {
            registrationRequest.AddEnrollment(
                new Enrollment(
                    student.Id,
                    semesterCourseId,
                    registrationRequestId));
        }

        context.RegistrationRequests.Add(registrationRequest);

        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(
            registrationRequestId,
            "Registration request submitted successfully.");
    }
}