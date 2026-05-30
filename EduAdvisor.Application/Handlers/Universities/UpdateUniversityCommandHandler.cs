using EduAdvisor.Application.Commands.Universities;
using EduAdvisor.Application.DTO.Universities;

namespace EduAdvisor.Application.Handlers.Universities;

public sealed class UpdateUniversityCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateUniversityCommand, Result<UniversityResponse>>
{
    #region Handle

    public async Task<Result<UniversityResponse>> Handle(
        UpdateUniversityCommand request,
        CancellationToken cancellationToken)
    {
        #region Fetch

        var university = await db.Universities
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (university is null)
            return Result<UniversityResponse>.NotFound("University not found.");

        #endregion

        #region Validate Name Uniqueness (if sent)

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var nameExists = await db.Universities
                .AnyAsync(x =>
                    x.Id != request.Id &&
                    x.Name == request.Name.Trim(),
                    cancellationToken);

            if (nameExists)
                return Result<UniversityResponse>.Conflict(
                    "A university with this name already exists.");

            university.UpdateName(request.Name);
        }

        #endregion

        #region Update Fields

        if (request.Description is not null)
            university.UpdateDescription(request.Description);

        if (request.Email is not null ||
            request.PhoneNumber is not null ||
            request.Website is not null)
        {
            university.UpdateContact(
                request.Email ?? university.Email,
                request.PhoneNumber ?? university.PhoneNumber,
                request.Website ?? university.Website);
        }

        if (!string.IsNullOrWhiteSpace(request.Address))
            university.UpdateAddress(request.Address);

        #endregion

        #region Save

        await db.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<UniversityResponse>.Success(
            MapToResponse(university),
            "University updated successfully.");
    }

    #endregion

    private static UniversityResponse MapToResponse(
        EduAdvisor.Domain.Entities.Universities.University u) =>
        new(u.Id, u.Name, u.Description, u.Address,
            u.Email, u.Website, u.PhoneNumber,
            u.IsActive, u.Faculties.Count,
            u.CreatedAt, u.UpdatedAt);
}