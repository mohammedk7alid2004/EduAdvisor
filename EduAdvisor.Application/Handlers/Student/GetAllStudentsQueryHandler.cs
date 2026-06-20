using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.Student;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.Student;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Student;

public sealed class GetAllStudentsQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetAllStudentsQuery,
        Result<PaginatedList<StudentResponseDto>>>
{
    public async Task<Result<PaginatedList<StudentResponseDto>>> Handle(
        GetAllStudentsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Students
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.StudentCode.Contains(request.Search) ||
                x.User.FullName.Contains(request.Search) ||
                x.User.Email!.Contains(request.Search));
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(x =>
                x.DepartmentId == request.DepartmentId);
        }

        if (request.AdvisorId.HasValue)
        {
            query = query.Where(x =>
                x.AdvisorId == request.AdvisorId);
        }

        if (request.AcademicYear.HasValue)
        {
            query = query.Where(x =>
                x.AcademicYear == request.AcademicYear);
        }

        var projected = query
            .OrderBy(x => x.StudentCode)
            .Select(x => new StudentResponseDto(
                x.Id,
                x.StudentCode,
                x.User.FullName,
                x.User.Email!,
                x.User.ProfileImageUrl,
                x.GPA,
                x.AcademicYear,
                x.CompletedHours,
                x.Department.Name,
                x.Advisor != null
                    ? x.Advisor.User.FullName
                    : null));

        var result = await PaginatedList<StudentResponseDto>
            .CreateAsync(
                projected,
                request.PageNumber,
                request.PageSize);

        return Result<PaginatedList<StudentResponseDto>>
            .Success(result);
    }
}