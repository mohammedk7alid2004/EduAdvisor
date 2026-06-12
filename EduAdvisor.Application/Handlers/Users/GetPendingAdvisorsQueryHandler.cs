using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.User;
using EduAdvisor.Application.Queries.Users;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Users;

public sealed class GetPendingAdvisorsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPendingAdvisorsQuery, Result<PaginatedList<PendingAdvisorDto>>>
{
    public async Task<Result<PaginatedList<PendingAdvisorDto>>> Handle(
        GetPendingAdvisorsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Advisors
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Department)
            .Where(a => a.IsPending);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(a =>
                a.User.Email!.ToLower().Contains(search) ||
                a.User.FullName.ToLower().Contains(search) ||
                a.Department.Name.ToLower().Contains(search));
        }

        var projected = query.Select(a => new PendingAdvisorDto(
            a.Id,
            a.User.FullName,
            a.User.Email!,
            a.Department.Name,
            a.CreatedAt));

        var result = await PaginatedList<PendingAdvisorDto>.CreateAsync(
            projected,
            request.PageNumber,
            request.PageSize
            );

        return Result<PaginatedList<PendingAdvisorDto>>.Success(result);
    }
}