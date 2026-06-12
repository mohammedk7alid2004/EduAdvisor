using EduAdvisor.Application.Commands.Departments;
using EduAdvisor.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.Departments;

public class DeleteDepartmentCommandHandler(
    IApplicationDbContext context,
    IStringLocalizer localizer)
    : IRequestHandler<DeleteDepartmentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        #region Fetch Department

        var department = await context.Departments
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (department is null)
            return Result<bool>.Failure(localizer["DepartmentNotFound"], 404);

        #endregion

        #region Check Dependencies

        var hasStudents = await context.Students
            .AnyAsync(x => x.DepartmentId == request.Id, cancellationToken);

        if (hasStudents)
            return Result<bool>.Failure(localizer["DepartmentHasStudents"], 400);

        var hasCourses = await context.Courses
            .AnyAsync(x => x.DepartmentId == request.Id, cancellationToken);

        if (hasCourses)
            return Result<bool>.Failure(localizer["DepartmentHasCourses"], 400);

        #endregion

        #region Delete & Save

        context.Departments.Remove(department);
        await context.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<bool>.Success(true, localizer["DepartmentDeletedSuccessfully"]);
    }
}