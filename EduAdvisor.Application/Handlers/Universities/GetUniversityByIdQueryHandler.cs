using EduAdvisor.Application.DTO.Universities;
using EduAdvisor.Application.Queries.Universities;

namespace EduAdvisor.Application.Handlers.Universities;

public sealed class GetUniversityByIdQueryHandler(IApplicationDbContext db)
  : IRequestHandler<GetUniversityByIdQuery, Result<UniversityResponse>>
{
    #region Handle

    public async Task<Result<UniversityResponse>> Handle(
        GetUniversityByIdQuery request,
        CancellationToken cancellationToken)
    {
        var response = await db.Universities
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new UniversityResponse(
                x.Id,
                x.Name,
                x.Description,
                x.Address,
                x.Email,
                x.Website,
                x.PhoneNumber,
                x.IsActive,
                x.Faculties.Count,
                x.CreatedAt,
                x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
            return Result<UniversityResponse>.NotFound("University not found.");

        return Result<UniversityResponse>.Success(response);
    }

    #endregion
}
