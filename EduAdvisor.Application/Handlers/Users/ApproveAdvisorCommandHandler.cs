using EduAdvisor.Application.Commands.Users;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Users;

public sealed class ApproveAdvisorCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ApproveAdvisorCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ApproveAdvisorCommand request,
        CancellationToken cancellationToken)
    {
        var advisor = await context.Advisors
            .FirstOrDefaultAsync(a => a.Id == request.AdvisorId, cancellationToken);

        if (advisor is null)
            return Result<bool>.NotFound("Advisor not found.");

        if (!advisor.IsPending)
            return Result<bool>.Conflict("Advisor is already approved.");

        advisor.Approve();

        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Advisor approved successfully.");
    }
}