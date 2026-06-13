using EduAdvisor.Application.Commands.SemesterCourses;
using EduAdvisor.Domain.Entities.AcademicModule;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.SemesterCourses;

public sealed class CreateBulkSemesterCoursesCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateBulkSemesterCoursesCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        CreateBulkSemesterCoursesCommand request,
        CancellationToken cancellationToken)
    {
        var semesterExists = await context.Semesters
            .AsNoTracking()
            .AnyAsync(s => s.Id == request.SemesterId && s.IsActive, cancellationToken);

        if (!semesterExists)
            return Result<bool>.NotFound("Active semester not found.");

        var foundPlanIds = await context.CourseAcademicPlans
            .AsNoTracking()
            .Where(p => request.CourseAcademicPlanIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        var missingPlanIds = request.CourseAcademicPlanIds
            .Except(foundPlanIds)
            .ToList();

        if (missingPlanIds.Count != 0)
            return Result<bool>.NotFound(
                $"The following course academic plans were not found: {string.Join(", ", missingPlanIds)}");

        var existingPlanIds = await context.SemesterCourses
            .AsNoTracking()
            .Where(sc =>
                sc.SemesterId == request.SemesterId &&
                request.CourseAcademicPlanIds.Contains(sc.CourseAcademicPlanId))
            .Select(sc => sc.CourseAcademicPlanId)
            .ToListAsync(cancellationToken);

        if (existingPlanIds.Count != 0)
            return Result<bool>.Conflict(
                $"The following plans are already assigned to this semester: {string.Join(", ", existingPlanIds)}");

        var semesterCourses = request.CourseAcademicPlanIds
            .Select(planId => new SemesterCourse(request.SemesterId, planId))
            .ToList();

        context.SemesterCourses.AddRange(semesterCourses);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, $"{semesterCourses.Count} semester courses created successfully.");
    }
}