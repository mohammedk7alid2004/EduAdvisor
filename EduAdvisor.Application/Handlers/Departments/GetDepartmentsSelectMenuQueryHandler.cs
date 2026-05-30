using EduAdvisor.Application.DTO.Common;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.Departments;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.Departments;

public class GetDepartmentsSelectMenuQueryHandler(
    IApplicationDbContext context,
    IStringLocalizer localizer)
    : IRequestHandler<GetDepartmentsSelectMenuQuery, Result<List<SelectMenuResponse>>>
{
    public async Task<Result<List<SelectMenuResponse>>> Handle(
        GetDepartmentsSelectMenuQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Departments.AsNoTracking().AsQueryable();

        if (request.FacultyId.HasValue)
            query = query.Where(x => x.FacultyId == request.FacultyId.Value);

        var result = await query
            .OrderBy(x => x.Name)
            .Select(x => new SelectMenuResponse(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        return Result<List<SelectMenuResponse>>
            .Success(result, localizer["DepartmentsRetrievedSuccessfully"]);
    }
}