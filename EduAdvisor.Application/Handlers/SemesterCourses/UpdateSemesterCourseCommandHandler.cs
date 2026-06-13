using EduAdvisor.Application.Commands.SemesterCourses;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.SemesterCourses;

public sealed class UpdateSemesterCourseCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateSemesterCourseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateSemesterCourseCommand request,
        CancellationToken cancellationToken)
    {
        var semesterCourse = await context.SemesterCourses
            .FirstOrDefaultAsync(sc => sc.Id == request.Id, cancellationToken);

        if (semesterCourse is null)
            return Result<bool>.NotFound("Semester course not found.");

        var semesterExists = await context.Semesters
            .AsNoTracking()
            .AnyAsync(s => s.Id == request.SemesterId && s.IsActive, cancellationToken);

        if (!semesterExists)
            return Result<bool>.NotFound("Active semester not found.");

        var planExists = await context.CourseAcademicPlans
            .AsNoTracking()
            .AnyAsync(p => p.Id == request.CourseAcademicPlanId, cancellationToken);

        if (!planExists)
            return Result<bool>.NotFound("Course academic plan not found.");

        var duplicateExists = await context.SemesterCourses
            .AsNoTracking()
            .AnyAsync(sc =>
                sc.Id != request.Id &&
                sc.SemesterId == request.SemesterId &&
                sc.CourseAcademicPlanId == request.CourseAcademicPlanId,
                cancellationToken);

        if (duplicateExists)
            return Result<bool>.Conflict(
                "This course is already assigned to the selected semester.");

        semesterCourse.Update(request.SemesterId, request.CourseAcademicPlanId);

        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Semester course updated successfully.");
    }
}