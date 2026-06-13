using EduAdvisor.Application.Commands.Semesters;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Semesters;

public sealed class ToggleSemesterRegistrationCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ToggleSemesterRegistrationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ToggleSemesterRegistrationCommand request,
        CancellationToken cancellationToken)
    {
        var semester = await context.Semesters
            .FirstOrDefaultAsync(s => s.Id == request.SemesterId, cancellationToken);

        if (semester is null)
            return Result<bool>.NotFound("Semester not found.");

        if (!semester.IsActive)
            return Result<bool>.Conflict("Cannot toggle registration on an inactive semester.");

        if (semester.IsRegistrationOpen)
            semester.CloseRegistration();
        else
            semester.OpenRegistration();

        await context.SaveChangesAsync(cancellationToken);

        var message = semester.IsRegistrationOpen
            ? "Registration opened successfully."
            : "Registration closed successfully.";

        return Result<bool>.Success(true, message);
    }
}