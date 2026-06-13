using EduAdvisor.Application.Commands.SemesterCourses;
using EduAdvisor.Domain.Entities.AcademicModule;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.SemesterCourses;

public sealed class CreateSemesterCourseCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateSemesterCourseCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateSemesterCourseCommand request,
        CancellationToken cancellationToken)
    {
        var semesterExists = await context.Semesters
            .AsNoTracking()
            .AnyAsync(s => s.Id == request.SemesterId && s.IsActive, cancellationToken);

        if (!semesterExists)
            return Result<Guid>.NotFound("Active semester not found.");

        var planExists = await context.CourseAcademicPlans
            .AsNoTracking()
            .AnyAsync(p => p.Id == request.CourseAcademicPlanId, cancellationToken);

        if (!planExists)
            return Result<Guid>.NotFound("Course academic plan not found.");

        var duplicateExists = await context.SemesterCourses
            .AsNoTracking()
            .AnyAsync(sc =>
                sc.SemesterId == request.SemesterId &&
                sc.CourseAcademicPlanId == request.CourseAcademicPlanId,
                cancellationToken);

        if (duplicateExists)
            return Result<Guid>.Conflict(
                "This course is already assigned to the selected semester.");

        var semesterCourse = new SemesterCourse(
            request.SemesterId,
            request.CourseAcademicPlanId);

        context.SemesterCourses.Add(semesterCourse);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(semesterCourse.Id, "Semester course created successfully.");
    }
}