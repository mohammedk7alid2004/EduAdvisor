using EduAdvisor.Application.Commands.Semesters;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Semesters;

public sealed class UpdateSemesterCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateSemesterCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateSemesterCommand request,
        CancellationToken cancellationToken)
    {
        var semester = await context.Semesters
            .FirstOrDefaultAsync(s => s.Id == request.SemesterId, cancellationToken);

        if (semester is null)
            return Result<bool>.NotFound("Semester not found.");

        var duplicateExists = await context.Semesters
            .AsNoTracking()
            .AnyAsync(s =>
                s.Id != request.SemesterId &&
                s.Name == request.Name.Trim() &&
                s.Year == request.Year &&
                s.StandardSemesterNumber == request.StandardSemesterNumber,
                cancellationToken);

        if (duplicateExists)
            return Result<bool>.Conflict(
                "A semester with the same name, year, and term already exists.");

        semester.UpdateName(request.Name);
        semester.UpdateYear(request.Year);
        semester.UpdateStartDate(request.StartDate);
        semester.UpdateEndDate(request.EndDate);
        semester.UpdateStandardSemesterNumber(request.StandardSemesterNumber);

        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Semester updated successfully.");
    }
}