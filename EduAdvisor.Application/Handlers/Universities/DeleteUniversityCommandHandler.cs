using EduAdvisor.Application.Commands.Universities;

namespace EduAdvisor.Application.Handlers.Universities;

public sealed class DeleteUniversityCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteUniversityCommand, Result<bool>>
{
    #region Handle

    public async Task<Result<bool>> Handle(
        DeleteUniversityCommand request,
        CancellationToken cancellationToken)
    {
        #region Fetch

        var university = await db.Universities
            .Include(x => x.Faculties)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (university is null)
            return Result<bool>.NotFound("University not found.");

        #endregion

        #region Check Faculties

        if (university.Faculties.Count > 0)
            return Result<bool>.Conflict(
                $"Cannot delete university with {university.Faculties.Count} faculty(s). Remove them first.");

        #endregion

        #region Delete & Save

        db.Universities.Remove(university);
        await db.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<bool>.Success(true, "University deleted successfully.");
    }

    #endregion
}