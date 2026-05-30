using System;
using EduAdvisor.Application.Commands.Faculties;

namespace EduAdvisor.Application.Handlers.Faculties;

public sealed class ToggleFacultyStatusCommandHandler(IApplicationDbContext db)
 : IRequestHandler<ToggleFacultyStatusCommand, Result<bool>>
{
    #region Handle

    public async Task<Result<bool>> Handle(
        ToggleFacultyStatusCommand request,
        CancellationToken cancellationToken)
    {
        #region Fetch

        var faculty = await db.Faculties
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (faculty is null)
            return Result<bool>.Failure("Faculty not found.", 404);

        #endregion

        #region Toggle & Save

        if (faculty.IsActive)
            faculty.Deactivate();
        else
            faculty.Activate();

        await db.SaveChangesAsync(cancellationToken);

        #endregion

        var message = faculty.IsActive
            ? "Faculty activated successfully."
            : "Faculty deactivated successfully.";

        return Result<bool>.Success(true, message);
    }

    #endregion
}
