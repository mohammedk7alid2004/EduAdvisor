using EduAdvisor.Application.DTO.Faculties;
using EduAdvisor.Application.Queries.Faculties;

namespace EduAdvisor.Application.Handlers.Faculties;

public sealed class GetAllFacultiesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllFacultiesQuery, Result<PaginatedList<FacultyListResponse>>>
{
    #region Handle

    public async Task<Result<PaginatedList<FacultyListResponse>>> Handle(
        GetAllFacultiesQuery request,
        CancellationToken cancellationToken)
    {
        #region Build Query

        var query = db.Faculties.AsNoTracking().AsQueryable();

        #endregion

        #region Filters

        if (request.UniversityId.HasValue)
            query = query.Where(x => x.UniversityId == request.UniversityId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x =>
                x.Name.Contains(request.Search) ||
                (x.Abbreviation != null && x.Abbreviation.Contains(request.Search)));

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        #endregion

        #region Projection

        var projected = query
            .OrderBy(x => x.Name)
            .Select(x => new FacultyListResponse(
                x.Id,
                x.UniversityId,
                x.University.Name,
                x.Name,
                x.Abbreviation,
                x.Email,
                x.IsActive,
                x.Departments.Count));

        #endregion

        #region Paginate

        var result = await PaginatedList<FacultyListResponse>
            .CreateAsync(projected, request.PageNumber, request.PageSize);

        #endregion

        return Result<PaginatedList<FacultyListResponse>>.Success(result);
    }

    #endregion
}
