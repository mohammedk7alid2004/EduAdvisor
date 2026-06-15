using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.User;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.RegistrationRequests;
using EduAdvisor.Domain.Enums.University;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.RegistrationRequest;

public sealed class GetMyRegistrationRequestsQueryHandler(
    IApplicationDbContext context,
    IGetCurrentUserRepository currentUser)
    : IRequestHandler<GetMyRegistrationRequestsQuery,
        Result<List<RegistrationRequestListDto>>>
{
    public async Task<Result<List<RegistrationRequestListDto>>> Handle(
        GetMyRegistrationRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Result<List<RegistrationRequestListDto>>
                .Unauthorized("User is not authenticated.");

        var studentId = await context.Students
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentId == Guid.Empty)
            return Result<List<RegistrationRequestListDto>>
                .NotFound("Student not found.");

        var requests = await context.RegistrationRequests
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.SubmittedAt)
            .Select(x => new RegistrationRequestListDto
            {
                Id = x.Id,
                SemesterName = x.Semester.Name,
                Status = x.Status.ToString(),
                SubmittedAt = x.SubmittedAt,
                Notes = x.Notes,
                CoursesCount = x.Enrollments.Count
            })
            .ToListAsync(cancellationToken);

        return Result<List<RegistrationRequestListDto>>
            .Success(requests);
    }
}