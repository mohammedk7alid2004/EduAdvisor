using EduAdvisor.Application.Commands.Faculties;
using EduAdvisor.Application.DTO.Faculties;
using EduAdvisor.Domain.Entities.Faculties;

namespace EduAdvisor.Application.Handlers.Faculties;

public sealed class CreateFacultyCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateFacultyCommand, Result<FacultyResponse>>
{
    #region Handle

    public async Task<Result<FacultyResponse>> Handle(
        CreateFacultyCommand request,
        CancellationToken cancellationToken)
    {
        #region Validate University

        var university = await db.Universities
            .FirstOrDefaultAsync(x => x.Id == request.UniversityId, cancellationToken);

        if (university is null)
            return Result<FacultyResponse>.Failure("University not found.", 404);

        #endregion

        #region Validate Uniqueness

        var exists = await db.Faculties
            .AnyAsync(x =>
                x.UniversityId == request.UniversityId &&
                x.Name == request.Name.Trim(),
                cancellationToken);

        if (exists)
            return Result<FacultyResponse>.Failure(
                "A faculty with this name already exists in this university.", 409);

        #endregion

        #region Create & Save

        var faculty = new Faculty(
            request.Name,
            request.UniversityId,
            request.Abbreviation,
            request.Email,
            request.Website,
            request.Description);

        await db.Faculties.AddAsync(faculty, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<FacultyResponse>.Success(
            MapToResponse(faculty, university.Name),
            "Faculty created successfully.",
            201);
    }

    #endregion

    private static FacultyResponse MapToResponse(Faculty f, string universityName) =>
        new(f.Id, f.UniversityId, universityName,
            f.Name, f.Abbreviation, f.Description,
            f.Email, f.Website, f.LogoUrl,
            f.IsActive, f.Departments.Count, f.Subjects.Count,
            f.CreatedAt, f.UpdatedAt);
}

