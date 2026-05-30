using EduAdvisor.Application.Commands.Departments;
using EduAdvisor.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.Departments;

public class UpdateDepartmentCommandHandler(
    IApplicationDbContext context,
    IStringLocalizer localizer)
    : IRequestHandler<UpdateDepartmentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        #region Fetch Department

        var department = await context.Departments
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (department is null)
            return Result<bool>.Failure(localizer["DepartmentNotFound"], 404);

        #endregion

        #region Validate Faculty (if sent)

        if (request.FacultyId.HasValue)
        {
            var facultyExists = await context.Faculties
                .AnyAsync(x => x.Id == request.FacultyId.Value, cancellationToken);

            if (!facultyExists)
                return Result<bool>.Failure(localizer["FacultyNotFound"], 404);
        }

        #endregion

        #region Validate Name Uniqueness (if sent)

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var nameExists = await context.Departments
                .AnyAsync(x =>
                    x.Id != request.Id &&
                    x.Name == request.Name.Trim() &&
                    x.FacultyId == (request.FacultyId ?? department.FacultyId),
                    cancellationToken);

            if (nameExists)
                return Result<bool>.Failure(localizer["DepartmentAlreadyExists"], 400);
        }

        #endregion

        #region Validate Code Uniqueness (if sent)

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var codeExists = await context.Departments
                .AnyAsync(x =>
                    x.Id != request.Id &&
                    x.Code == request.Code.Trim(),
                    cancellationToken);

            if (codeExists)
                return Result<bool>.Failure(localizer["DepartmentCodeAlreadyExists"], 400);
        }

        #endregion

        #region Update & Save

        if (!string.IsNullOrWhiteSpace(request.Name))
            department.UpdateName(request.Name);

        if (request.Code is not null)
            department.UpdateCode(request.Code);

        if (request.Description is not null)
            department.UpdateDescription(request.Description);

        if (request.FacultyId.HasValue)
            department.UpdateFaculty(request.FacultyId.Value);

        await context.SaveChangesAsync(cancellationToken);

        #endregion

        return Result<bool>.Success(true, localizer["DepartmentUpdatedSuccessfully"]);
    }
}