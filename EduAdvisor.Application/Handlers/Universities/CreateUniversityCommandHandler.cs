using EduAdvisor.Application.Commands.Universities;
using EduAdvisor.Application.DTO.Universities;
using EduAdvisor.Domain.Entities.Universities;

namespace EduAdvisor.Application.Handlers.Universities;

public sealed class CreateUniversityCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateUniversityCommand, Result<UniversityResponse>>
{
    #region Handle

    public async Task<Result<UniversityResponse>> Handle(
        CreateUniversityCommand request,
        CancellationToken cancellationToken)
    {
        #region Validate Uniqueness

        var exists = await db.Universities
            .AnyAsync(x => x.Name == request.Name.Trim(), cancellationToken);

        if (exists)
            return Result<UniversityResponse>.Conflict(
                "A university with this name already exists.");

        #endregion

        #region Create & Save

        var university = new University(
            request.Name,
            request.Description,
            request.Address,
            request.Email,
            request.Website,
            request.PhoneNumber);

        await db.Universities.AddAsync(university, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<UniversityResponse>.Success(
            MapToResponse(university),
            "University created successfully.",
            201);
    }

    #endregion

    private static UniversityResponse MapToResponse(University u) =>
        new(u.Id, u.Name, u.Description, u.Address,
            u.Email, u.Website, u.PhoneNumber,
            u.IsActive, u.Faculties.Count,
            u.CreatedAt, u.UpdatedAt);
}