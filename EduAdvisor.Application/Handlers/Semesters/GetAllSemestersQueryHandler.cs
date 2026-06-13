using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.Semesters;
using EduAdvisor.Application.Queries.Semesters;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Semesters;

public sealed class GetAllSemestersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllSemestersQuery, Result<PaginatedList<SemesterListDto>>>
{
    public async Task<Result<PaginatedList<SemesterListDto>>> Handle(
        GetAllSemestersQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Semesters
            .AsNoTracking()
            .Include(s => s.CreatedBy)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        if (request.IsRegistrationOpen.HasValue)
            query = query.Where(s => s.IsRegistrationOpen == request.IsRegistrationOpen.Value);

        if (request.StandardSemesterNumber.HasValue)
            query = query.Where(s => s.StandardSemesterNumber == request.StandardSemesterNumber.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(search));
        }

        var projected = query.Select(s => new SemesterListDto(
            s.Id,
            s.Name,
            s.Year,
            s.StartDate,
            s.EndDate,
            s.IsActive,
            s.IsRegistrationOpen,
            s.StandardSemesterNumber,
            s.CreatedBy != null ? s.CreatedBy.UserName : null,
            s.CreatedAt));

        var result = await PaginatedList<SemesterListDto>.CreateAsync(
            projected,
            request.PageNumber,
            request.PageSize
            );

        return Result<PaginatedList<SemesterListDto>>.Success(result);
    }
}