using EduAdvisor.Application.Commands.Faculties;
using EduAdvisor.Application.DTO.Faculties;

namespace EduAdvisor.Application.Handlers.Faculties;

public sealed class UpdateFacultyCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateFacultyCommand, Result<FacultyResponse>>
{
    #region Handle

    public async Task<Result<FacultyResponse>> Handle(
        UpdateFacultyCommand request,
        CancellationToken cancellationToken)
    {
        #region Fetch

        var faculty = await db.Faculties
            .Include(x => x.University)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (faculty is null)
            return Result<FacultyResponse>.Failure("Faculty not found.", 404);

        #endregion

        #region Validate Name Uniqueness (if sent)

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var nameExists = await db.Faculties
                .AnyAsync(x =>
                    x.Id != request.Id &&
                    x.UniversityId == faculty.UniversityId &&
                    x.Name == request.Name.Trim(),
                    cancellationToken);

            if (nameExists)
                return Result<FacultyResponse>.Failure(
                    "A faculty with this name already exists in this university.", 409);

            faculty.UpdateName(request.Name);
        }

        #endregion

        #region Update Fields

        if (request.Description is not null) faculty.UpdateDescription(request.Description);
        if (request.Email is not null) faculty.UpdateEmail(request.Email);
        if (request.Website is not null) faculty.UpdateWebsite(request.Website);
        if (request.LogoUrl is not null) faculty.SetLogoUrl(request.LogoUrl);

        #endregion

        #region Save

        await db.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<FacultyResponse>.Success(
            MapToResponse(faculty),
            "Faculty updated successfully.");
    }

    #endregion

    private static FacultyResponse MapToResponse(
        EduAdvisor.Domain.Entities.Faculties.Faculty f) =>
        new(f.Id, f.UniversityId, f.University.Name,
            f.Name, f.Abbreviation, f.Description,
            f.Email, f.Website, f.LogoUrl,
            f.IsActive, f.Departments.Count,
            f.CreatedAt, f.UpdatedAt);
}
