using EduAdvisor.Application.Commands.Faculties;

namespace EduAdvisor.Application.Handlers.Faculties;

public sealed class DeleteFacultyCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteFacultyCommand, Result<bool>>
{
    #region Handle

    public async Task<Result<bool>> Handle(
        DeleteFacultyCommand request,
        CancellationToken cancellationToken)
    {
        #region Fetch

        var faculty = await db.Faculties
            .Include(x => x.Departments)
            .Include(x => x.Subjects)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (faculty is null)
            return Result<bool>.Failure("Faculty not found.", 404);

        #endregion

        #region Check Dependencies

        if (faculty.Departments.Count > 0)
            return Result<bool>.Failure(
                $"Cannot delete faculty with {faculty.Departments.Count} department(s). Remove them first.", 409);

        if (faculty.Subjects.Count > 0)
            return Result<bool>.Failure(
                $"Cannot delete faculty with {faculty.Subjects.Count} subject(s). Remove them first.", 409);

        #endregion

        #region Delete & Save

        db.Faculties.Remove(faculty);
        await db.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<bool>.Success(true, "Faculty deleted successfully.");
    }

    #endregion
}