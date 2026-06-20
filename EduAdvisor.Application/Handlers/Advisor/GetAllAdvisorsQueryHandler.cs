using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.Advisor;
using EduAdvisor.Application.Interfaces;
using EduAdvisorEduAdvisor.Application.Queries.AuthModules;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Advisors;

public sealed class GetAllAdvisorsQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetAllAdvisorsQuery, Result<PaginatedList<AdvisorResponseDto>>>
{
    public async Task<Result<PaginatedList<AdvisorResponseDto>>> Handle(
        GetAllAdvisorsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Advisors
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.User.FullName.Contains(request.Search) ||
                x.User.Email!.Contains(request.Search));
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(x =>
                x.DepartmentId == request.DepartmentId);
        }

        if (request.IsPending.HasValue)
        {
            query = query.Where(x =>
                x.IsPending == request.IsPending.Value);
        }

        var projected = query
            .OrderBy(x => x.User.FullName)
            .Select(x => new AdvisorResponseDto(
                x.Id,
                x.User.FullName,
                x.User.Email!,
                x.User.ProfileImageUrl,
                x.Department.Name,
                context.Students.Count(s => s.AdvisorId == x.Id),
                x.IsPending));

        var result = await PaginatedList<AdvisorResponseDto>
            .CreateAsync(
                projected,
                request.PageNumber,
                request.PageSize);

        return Result<PaginatedList<AdvisorResponseDto>>
            .Success(result);
    }
}