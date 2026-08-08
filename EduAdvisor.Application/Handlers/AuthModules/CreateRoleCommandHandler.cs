using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Domain.Entities.RoleModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace EduAdvisor.Application.Handlers.AuthModules;

public sealed class CreateRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    IApplicationDbContext context,
    IStringLocalizer localizer,
    ILogger<CreateRoleCommandHandler> logger)
    : IRequestHandler<CreateRoleCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var roleName = request.Name.Trim();

        var permissionIds = request.PermissionIds
            .Distinct()
            .ToHashSet();

        var existingRole = await roleManager.FindByNameAsync(roleName);

        if (existingRole is not null)
        {
            return Result<string>.Failure(
                localizer["RoleAlreadyExists"],
                StatusCodes.Status400BadRequest);
        }

        await using var transaction =
            await context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var role = new ApplicationRole
            {
                Name = roleName
            };

            var createResult = await roleManager.CreateAsync(role);

            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(
                        error => error.Description));

                return Result<string>.Failure(
                    string.Format(
                        localizer["FailedToCreateRole"],
                        errors),
                    StatusCodes.Status400BadRequest);
            }

            if (permissionIds.Count > 0)
            {
                var permissions = await context.Permissions
                    .AsNoTracking()
                    .Where(permission =>
                        permissionIds.Contains(
                            permission.PermissionId))
                    .ToListAsync(cancellationToken);

                var invalidPermissionIds = permissionIds
                    .Except(
                        permissions.Select(
                            permission =>
                                permission.PermissionId))
                    .ToList();

                if (invalidPermissionIds.Count > 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return Result<string>.Failure(
                        string.Format(
                            localizer["InvalidPermissionIds"],
                            string.Join(
                                ", ",
                                invalidPermissionIds)),
                        StatusCodes.Status400BadRequest);
                }

                var rolePermissions = permissions
                    .Select(permission =>
                        RolePermission.Create(
                            role.Id,
                            permission.PermissionId))
                    .ToList();

                await context.RolePermissions.AddRangeAsync(
                    rolePermissions,
                    cancellationToken);

                await context.SaveChangesAsync(
                    cancellationToken);
            }

            await transaction.CommitAsync(
                cancellationToken);

            return Result<string>.Success(
                role.Id,
                localizer["RoleCreatedSuccessfully"],
                StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while creating role '{RoleName}'.",
                roleName);

            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}