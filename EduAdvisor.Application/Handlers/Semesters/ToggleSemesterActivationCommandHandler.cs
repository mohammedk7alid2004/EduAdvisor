using EduAdvisor.Application.Commands.Semesters;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Semesters;

public sealed class ToggleSemesterActivationCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ToggleSemesterActivationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ToggleSemesterActivationCommand request,
        CancellationToken cancellationToken)
    {
        var semester = await context.Semesters
            .FirstOrDefaultAsync(s => s.Id == request.SemesterId, cancellationToken);

        if (semester is null)
            return Result<bool>.NotFound("Semester not found.");

        if (semester.IsActive)
            semester.Deactivate();
        else
            semester.Activate();

        await context.SaveChangesAsync(cancellationToken);

        var message = semester.IsActive
            ? "Semester activated successfully."
            : "Semester deactivated successfully.";

        return Result<bool>.Success(true, message);
    }
}