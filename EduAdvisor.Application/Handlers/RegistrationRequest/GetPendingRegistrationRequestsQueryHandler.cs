using EduAdvisor.Application.DTO.RegistrationRequest;
using EduAdvisor.Application.Queries.RegistrationRequests;
using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Application.Handlers.RegistrationRequest
{
    public sealed class GetPendingRegistrationRequestsQueryHandler(
        IApplicationDbContext context,
        IGetCurrentUserRepository currentUser)
        : IRequestHandler<
            GetPendingRegistrationRequestsQuery,
            Result<PaginatedList<PendingRegistrationRequestDto>>>
    {
        public async Task<Result<PaginatedList<PendingRegistrationRequestDto>>> Handle(
            GetPendingRegistrationRequestsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = currentUser.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Result<PaginatedList<PendingRegistrationRequestDto>>
                    .Unauthorized("User is not authenticated.");

            var advisor = await context.Advisors
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (advisor is null)
                return Result<PaginatedList<PendingRegistrationRequestDto>>
                    .NotFound("Advisor not found.");

            var query = context.RegistrationRequests
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Status == EnrollmentStatus.Pending &&
                    x.Student.AdvisorId == advisor.Id)
                .OrderByDescending(x => x.SubmittedAt)
                .Select(x => new PendingRegistrationRequestDto
                {
                    RegistrationRequestId = x.Id,
                    StudentId = x.StudentId,
                    StudentName = x.Student.User.FullName,
                    StudentCode = x.Student.StudentCode,
                    DepartmentName = x.Student.Department.Name,
                    AcademicYear = x.Student.AcademicYear,
                    StudentPhotoUrl = x.Student.User.ProfileImageUrl,
                    SubmittedAt = x.SubmittedAt,
                    CoursesCount = x.Enrollments.Count(),
                    Status = x.Status.ToString()
                });

            var result = await PaginatedList<PendingRegistrationRequestDto>
                .CreateAsync(
                    query,
                    request.PageNumber,
                    request.PageSize);

            if (result.TotalCount == 0)
            {
                return Result<PaginatedList<PendingRegistrationRequestDto>>
                    .Success(result, "No pending registration requests found.");
            }

            return Result<PaginatedList<PendingRegistrationRequestDto>>
                .Success(result, "Pending registration requests retrieved successfully.");
        }
    }
}