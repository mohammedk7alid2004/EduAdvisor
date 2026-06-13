using EduAdvisor.Application.Commands.CourseAcademicPlans;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseAcademicPlans;

public sealed class DeleteCourseAcademicPlansCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteCourseAcademicPlansCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteCourseAcademicPlansCommand request,
        CancellationToken cancellationToken)
    {
        var existingIds = await context.CourseAcademicPlans
            .AsNoTracking()
            .Where(p => request.Ids.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (existingIds.Count != request.Ids.Count)
        {
            var missingIds = request.Ids.Except(existingIds).ToList();
            return Result<bool>.NotFound(
                $"The following plans were not found: {string.Join(", ", missingIds)}");
        }

        await context.CourseAcademicPlans
            .Where(p => request.Ids.Contains(p.Id))
            .ExecuteDeleteAsync(cancellationToken);

        return Result<bool>.Success(true, "Course academic plans deleted successfully.");
    }
}