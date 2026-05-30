using EduAdvisor.Application.Commands.Universities;

namespace EduAdvisor.Application.Handlers.Universities;

public sealed class ToggleUniversityStatusCommandHandler(IApplicationDbContext db)
    : IRequestHandler<ToggleUniversityStatusCommand, Result<bool>>
{
    #region Handle

    public async Task<Result<bool>> Handle(
        ToggleUniversityStatusCommand request,
        CancellationToken cancellationToken)
    {
        #region Fetch

        var university = await db.Universities
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (university is null)
            return Result<bool>.NotFound("University not found.");

        #endregion

        #region Toggle & Save

        if (university.IsActive)
            university.Deactivate();
        else
            university.Activate();

        await db.SaveChangesAsync(cancellationToken);

        #endregion

        var message = university.IsActive
            ? "University activated successfully."
            : "University deactivated successfully.";

        return Result<bool>.Success(true, message);
    }

    #endregion
}