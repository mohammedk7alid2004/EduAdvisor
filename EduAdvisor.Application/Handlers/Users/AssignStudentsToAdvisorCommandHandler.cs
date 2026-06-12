using EduAdvisor.Application.Commands.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Users;

public sealed class AssignStudentsToAdvisorCommandHandler(IApplicationDbContext db)
    : IRequestHandler<AssignStudentsToAdvisorCommand, Result<bool>>
{
    #region Handle

    public async Task<Result<bool>> Handle(
        AssignStudentsToAdvisorCommand request,
        CancellationToken cancellationToken)
    {
        #region Fetch Advisor

        var advisor = await db.Advisors
            .FirstOrDefaultAsync(x => x.Id == request.AdvisorId, cancellationToken);

        if (advisor is null)
            return Result<bool>.NotFound("Advisor not found.");

        if (advisor.IsPending)
            return Result<bool>.Failure("Cannot assign students to a pending advisor.");

        #endregion

        #region Fetch Students

        var students = await db.Students
            .Where(x => request.StudentIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var notFoundIds = request.StudentIds
            .Except(students.Select(x => x.Id))
            .ToList();

        if (notFoundIds.Count > 0)
            return Result<bool>.NotFound("Some students were not found.");

        #endregion

        #region Validate — already assigned to another advisor

        var alreadyAssigned = students
            .Where(x => x.AdvisorId.HasValue && x.AdvisorId != request.AdvisorId)
            .Select(x => x.Id)
            .ToList();

        if (alreadyAssigned.Count > 0)
            return Result<bool>.Conflict(
                $"{alreadyAssigned.Count} student(s) are already assigned to another advisor.");

        #endregion

        #region Assign & Save

        foreach (var student in students)
            student.AssignAdvisor(request.AdvisorId);

        await db.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<bool>.Success(true,
            $"{students.Count} student(s) assigned to advisor successfully.");
    }

    #endregion
}