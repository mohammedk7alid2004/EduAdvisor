using EduAdvisor.Application.Commands.Semesters;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Semesters;

public sealed class DeleteSemestersCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteSemestersCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteSemestersCommand request,
        CancellationToken cancellationToken)
    {
        var existingIds = await context.Semesters
            .AsNoTracking()
            .Where(s => request.SemesterIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        if (existingIds.Count != request.SemesterIds.Count)
        {
            var missingIds = request.SemesterIds
                .Except(existingIds)
                .ToList();

            return Result<bool>.NotFound(
                $"The following semesters were not found: {string.Join(", ", missingIds)}");
        }

        await context.Semesters
            .Where(s => request.SemesterIds.Contains(s.Id))
            .ExecuteDeleteAsync(cancellationToken);

        return Result<bool>.Success(true, "Semesters deleted successfully.");
    }
}