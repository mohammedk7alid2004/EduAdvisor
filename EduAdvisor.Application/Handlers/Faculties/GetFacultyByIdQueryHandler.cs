using EduAdvisor.Application.DTO.Faculties;
using EduAdvisor.Application.Queries.Faculties;

namespace EduAdvisor.Application.Handlers.Faculties;

public sealed class GetFacultyByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetFacultyByIdQuery, Result<FacultyResponse>>
{
    #region Handle

    public async Task<Result<FacultyResponse>> Handle(
        GetFacultyByIdQuery request,
        CancellationToken cancellationToken)
    {
        var response = await db.Faculties
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new FacultyResponse(
                x.Id,
                x.UniversityId,
                x.University.Name,
                x.Name,
                x.Abbreviation,
                x.Description,
                x.Email,
                x.Website,
                x.LogoUrl,
                x.IsActive,
                x.Departments.Count,
                x.CreatedAt,
                x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
            return Result<FacultyResponse>.Failure("Faculty not found.", 404);

        return Result<FacultyResponse>.Success(response);
    }

    #endregion
}