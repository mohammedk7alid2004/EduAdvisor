using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.RegistrationRequest;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.RegistrationRequests;
using EduAdvisor.Domain.Entities.AcademicModule;
using EduAdvisor.Domain.Enums.University;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.RegistrationRequest;

public sealed class GetPendingRegistrationRequestsQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<
        GetPendingRegistrationRequestsQuery,
        Result<PaginatedList<PendingRegistrationRequestDto>>>
{
    public async Task<Result<PaginatedList<PendingRegistrationRequestDto>>> Handle(
        GetPendingRegistrationRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.RegistrationRequests
            .AsNoTracking()
            .Where(x => x.Status == EnrollmentStatus.Pending)
            .OrderByDescending(x => x.SubmittedAt)
            .Select(x => new PendingRegistrationRequestDto
            {
                RegistrationRequestId = x.Id,

                StudentId = x.StudentId,

                StudentName =
                    x.Student.User.FullName,

                StudentCode = x.Student.StudentCode,

                DepartmentName =
                    x.Student.Department.Name,

                AcademicYear =
                    x.Student.AcademicYear,

                StudentPhotoUrl =
                    x.Student.User.ProfileImageUrl,

                SubmittedAt =
                    x.SubmittedAt,

                CoursesCount =
                    x.Enrollments.Count,

                Status =
                    x.Status.ToString()
            });

        var paginatedResult =
            await PaginatedList<PendingRegistrationRequestDto>
                .CreateAsync(
                    query,
                    request.PageNumber,
                    request.PageSize);

        return Result<PaginatedList<PendingRegistrationRequestDto>>
            .Success(paginatedResult);
    }
}