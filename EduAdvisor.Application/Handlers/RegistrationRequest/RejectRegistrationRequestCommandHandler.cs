using EduAdvisor.Application.Commands.RegistrationRequests;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Domain.Enums.University;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.RegistrationRequest;

public sealed class RejectRegistrationRequestCommandHandler(
    IApplicationDbContext context,
    IGetCurrentUserRepository currentUser)
    : IRequestHandler<
        RejectRegistrationRequestCommand,
        Result<bool>>
{
    public async Task<Result<bool>> Handle(
        RejectRegistrationRequestCommand request,
        CancellationToken cancellationToken)
    {
        var advisorUserId = currentUser.GetUserId();

        if (string.IsNullOrWhiteSpace(advisorUserId))
            return Result<bool>.Unauthorized("Unauthorized.");

        var advisorId = await context.Advisors
            .Where(x => x.UserId == advisorUserId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (advisorId == Guid.Empty)
            return Result<bool>.NotFound("Advisor not found.");

        var registrationRequest = await context.RegistrationRequests
            .Include(x => x.Enrollments)
            .FirstOrDefaultAsync(
                x => x.Id == request.RegistrationRequestId,
                cancellationToken);

        if (registrationRequest is null)
            return Result<bool>.NotFound(
                "Registration request not found.");

        if (registrationRequest.Status != EnrollmentStatus.Pending)
            return Result<bool>.Conflict(
                "Request already processed.");

        registrationRequest.Reject(
            advisorId,
            request.Reason);

        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(
            true,
            "Registration request rejected successfully.");
    }
}