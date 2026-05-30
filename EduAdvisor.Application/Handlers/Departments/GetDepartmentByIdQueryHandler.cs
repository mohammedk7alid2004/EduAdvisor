using EduAdvisor.Application.DTO.Departments;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.Departments;
using MediatR;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.Departments;

public class GetDepartmentByIdQueryHandler(
    IApplicationDbContext context,
    IStringLocalizer localizer)
    : IRequestHandler<GetDepartmentByIdQuery, Result<DescriptionListResponse>>
{
    public async Task<Result<DescriptionListResponse>> Handle(
        GetDepartmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        #region Query & Map

        var response = await context.Departments
            .AsNoTracking()
            .Include(x => x.Faculty)
            .Where(x => x.Id == request.Id)
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
                x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        #endregion

        if (response is null)
            return Result<DescriptionListResponse>.Failure(localizer["DepartmentNotFound"], 404);

        return Result<DescriptionListResponse>
            .Success(response, localizer["DepartmentRetrievedSuccessfully"]);
    }
}