using EduAdvisor.Application.DTO.Departments;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.Departments;
using MediatR;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.Departments;

public class GetAllDepartmentsQueryHandler(
    IApplicationDbContext context,
    IStringLocalizer localizer)
    : IRequestHandler<GetAllDepartmentsQuery, Result<PaginatedList<DescriptionListResponse>>>
{
    public async Task<Result<PaginatedList<DescriptionListResponse>>> Handle(
        GetAllDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        #region Build Query

        var query = context.Departments
            .AsNoTracking()
            .Include(x => x.Faculty)
            .AsQueryable();

        #endregion

        #region Filters

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x =>
                x.Name.Contains(request.Search) ||
                (x.Code != null && x.Code.Contains(request.Search)));

        if (request.FacultyId.HasValue)
            query = query.Where(x => x.FacultyId == request.FacultyId.Value);

        #endregion

        #region Projection

        var projected = query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new DescriptionListResponse(
                x.Id,
                x.FacultyId,
                x.Faculty.Name,
                x.Name,
                x.Description ?? string.Empty,
                x.Code ?? string.Empty,
                x.CreatedBy != null ? x.CreatedBy.FullName : string.Empty,
                x.UpdatedBy != null ? x.UpdatedBy.FullName : string.Empty,
                x.CreatedAt,
                x.UpdatedAt));

        #endregion

        #region Paginate

        var result = await PaginatedList<DescriptionListResponse>
            .CreateAsync(projected, request.PageNumber, request.PageSize);

        #endregion

        return Result<PaginatedList<DescriptionListResponse>>
            .Success(result, localizer["DepartmentsRetrievedSuccessfully"]);
    }
}