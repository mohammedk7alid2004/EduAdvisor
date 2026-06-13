using EduAdvisor.Application.Commands.Semesters;
using EduAdvisor.Domain.Entities.Semesters;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Semesters;

public sealed class CreateSemesterCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateSemesterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateSemesterCommand request,
        CancellationToken cancellationToken)
    {
        var duplicateExists = await context.Semesters
            .AsNoTracking()
            .AnyAsync(s =>
                s.Name == request.Name.Trim() &&
                s.Year == request.Year &&
                s.StandardSemesterNumber == request.StandardSemesterNumber,
                cancellationToken);

        if (duplicateExists)
            return Result<Guid>.Conflict(
                "A semester with the same name, year, and term already exists.");

        var semester = new Semester(
            request.Name,
            request.Year,
            request.StartDate,
            request.EndDate,
            request.StandardSemesterNumber);

        context.Semesters.Add(semester);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(semester.Id, "Semester created successfully.");
    }
}