using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.RegistrationRequest;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.RegistrationRequests;
using EduAdvisor.Domain.Enums.University;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.RegistrationRequest;

public sealed class GetAdvisorProcessedRequestsQueryHandler(
    IApplicationDbContext context,
    IGetCurrentUserRepository currentUser)
    : IRequestHandler<
        GetAdvisorProcessedRequestsQuery,
        Result<List<AdvisorProcessedRequestDto>>>
{
    public async Task<Result<List<AdvisorProcessedRequestDto>>> Handle(
        GetAdvisorProcessedRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<List<AdvisorProcessedRequestDto>>.Failure(
                "Unauthorized.",
                StatusCodes.Status401Unauthorized);
        }

        if (request.Status.HasValue &&
            request.Status.Value is not EnrollmentStatus.Approved &&
            request.Status.Value is not EnrollmentStatus.Rejected)
        {
            return Result<List<AdvisorProcessedRequestDto>>.Failure(
                "Status must be Approved or Rejected.",
                StatusCodes.Status400BadRequest);
        }

        var advisorId = await context.Advisors
            .AsNoTracking()
            .Where(advisor => advisor.UserId == userId)
            .Select(advisor => advisor.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (advisorId == Guid.Empty)
        {
            return Result<List<AdvisorProcessedRequestDto>>.Failure(
                "Advisor not found.",
                StatusCodes.Status404NotFound);
        }

        var query = context.RegistrationRequests
            .AsNoTracking()
            .Where(registrationRequest =>
                registrationRequest.ReviewedByAdvisorId == advisorId &&
                (registrationRequest.Status ==
                    EnrollmentStatus.Approved ||
                 registrationRequest.Status ==
                    EnrollmentStatus.Rejected));

        if (request.Status.HasValue)
        {
            var requestedStatus = request.Status.Value;

            query = query.Where(registrationRequest =>
                registrationRequest.Status == requestedStatus);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(registrationRequest =>
                registrationRequest.Student.User.FullName.Contains(search) ||
                registrationRequest.Student.StudentCode.Contains(search));
        }

        var processedRequests = await query
            .OrderByDescending(registrationRequest =>
                registrationRequest.SubmittedAt)
            .Select(registrationRequest =>
                new AdvisorProcessedRequestDto(
                    registrationRequest.Id,
                    registrationRequest.StudentId,
                    registrationRequest.Student.User.FullName,
                    registrationRequest.Student.StudentCode,
                    registrationRequest.Student.Department.Name,
                    registrationRequest.SemesterId,
                    registrationRequest.Status.ToString(),
                    registrationRequest.Notes,
                    registrationRequest.SubmittedAt,
                    registrationRequest.Enrollments.Count))
            .ToListAsync(cancellationToken);

        return Result<List<AdvisorProcessedRequestDto>>.Success(
            processedRequests,
            "Processed registration requests retrieved successfully.");
    }
}