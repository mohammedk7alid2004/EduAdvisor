using EduAdvisor.Application.Commands.CourseModules;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseModules;

public sealed class DeleteCoursesCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteCoursesCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteCoursesCommand request,
        CancellationToken cancellationToken)
    {
        var existingIds = await context.Courses
            .AsNoTracking()
            .Where(c => request.CourseIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        if (existingIds.Count != request.CourseIds.Count)
        {
            var missingIds = request.CourseIds
                .Except(existingIds)
                .ToList();

            return Result<bool>.NotFound(
                $"The following courses were not found: {string.Join(", ", missingIds)}");
        }

        await context.CoursePrerequisites
            .Where(cp =>
                request.CourseIds.Contains(cp.CourseId) ||
                request.CourseIds.Contains(cp.PrerequisiteCourseId))
            .ExecuteDeleteAsync(cancellationToken);

        await context.Courses
            .Where(c => request.CourseIds.Contains(c.Id))
            .ExecuteDeleteAsync(cancellationToken);

        return Result<bool>.Success(true, "Courses deleted successfully.");
    }
}