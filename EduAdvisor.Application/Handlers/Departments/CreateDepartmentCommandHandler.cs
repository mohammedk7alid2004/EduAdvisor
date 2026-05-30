using EduAdvisor.Application.Commands.Departments;
using EduAdvisor.Application.DTO.Departments;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Domain.Entities.Departments;
using MediatR;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.Departments;

public class CreateDepartmentCommandHandler(
    IApplicationDbContext context,
    IStringLocalizer localizer)
    : IRequestHandler<CreateDepartmentCommand, Result<DepartmentsResponse>>
{
    public async Task<Result<DepartmentsResponse>> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        #region Validate Faculty

        var faculty = await context.Faculties
            .FirstOrDefaultAsync(x => x.Id == request.FacultyId, cancellationToken);

        if (faculty is null)
            return Result<DepartmentsResponse>.Failure(localizer["FacultyNotFound"], 404);

        #endregion

        #region Validate Name Uniqueness

        var nameExists = await context.Departments
            .AnyAsync(x => x.Name == request.Name.Trim() && x.FacultyId == request.FacultyId,
                cancellationToken);

        if (nameExists)
            return Result<DepartmentsResponse>.Failure(localizer["DepartmentAlreadyExists"], 400);

        #endregion

        #region Validate Code Uniqueness

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var codeExists = await context.Departments
                .AnyAsync(x => x.Code == request.Code.Trim(), cancellationToken);

            if (codeExists)
                return Result<DepartmentsResponse>.Failure(localizer["DepartmentCodeAlreadyExists"], 400);
        }

        #endregion

        #region Create & Save

        var department = new Department(
            request.Name,
            request.FacultyId,
            request.Code,
            request.Description);

        await context.Departments.AddAsync(department, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        #endregion

        #region Map Response

        var response = new DepartmentsResponse(
            department.Id,
            department.FacultyId,
            faculty.Name,
            department.Name,
            department.Code,
            department.Description,
            true,
            0,
            department.CreatedAt,
            department.UpdatedAt);

        #endregion

        return Result<DepartmentsResponse>.Success(response, localizer["DepartmentCreatedSuccessfully"], 201);
    }
}