using EduAdvisor.Application.DTO.Universities;
using EduAdvisor.Application.Queries.Universities;

namespace EduAdvisor.Application.Handlers.Universities;

public sealed class GetAllUniversitiesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllUniversitiesQuery, Result<PaginatedList<UniversityListResponse>>>
{
    #region Handle

    public async Task<Result<PaginatedList<UniversityListResponse>>> Handle(
        GetAllUniversitiesQuery request,
        CancellationToken cancellationToken)
    {
        #region Build Query

        var query = db.Universities.AsNoTracking().AsQueryable();

        #endregion

        #region Filters

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x =>
                x.Name.Contains(request.Search) ||
                (x.Email != null && x.Email.Contains(request.Search)));

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        #endregion

        #region Projection

        var projected = query
            .OrderBy(x => x.Name)
            .Select(x => new UniversityListResponse(
                x.Id,
                x.Name,
                x.Email,
                x.Website,
                x.IsActive,
                x.Faculties.Count));

        #endregion

        #region Paginate

        var result = await PaginatedList<UniversityListResponse>
            .CreateAsync(projected, request.PageNumber, request.PageSize);

        #endregion

        return Result<PaginatedList<UniversityListResponse>>.Success(result);
    }

    #endregion
}
