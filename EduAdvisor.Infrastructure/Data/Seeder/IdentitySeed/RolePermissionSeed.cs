using EduAdvisor.Application.Common.Abstractions.Consts;
using EduAdvisor.Domain.Entities.RoleModule;
using EduAdvisor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Infrastructure.Data.Seeder.IdentitySeed;

public static class RolePermissionSeed
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager)
    {
        if (await context.RolePermissions.AnyAsync())
        {
            return;
        }

        var adminRole = await roleManager.FindByNameAsync("Admin");
        var studentRole = await roleManager.FindByNameAsync("Student");
        var advisorRole = await roleManager.FindByNameAsync("Advisor");

        if (adminRole is null)
        {
            throw new InvalidOperationException(
                "Admin role was not found.");
        }

        if (studentRole is null)
        {
            throw new InvalidOperationException(
                "Student role was not found.");
        }

        if (advisorRole is null)
        {
            throw new InvalidOperationException(
                "Advisor role was not found.");
        }

        var permissions = await context.Permissions
            .AsNoTracking()
            .ToListAsync();

        #region Admin

        var adminPermissions = permissions
            .Select(permission =>
                RolePermission.Create(
                    adminRole.Id,
                    permission.PermissionId))
            .ToList();

        context.RolePermissions.AddRange(adminPermissions);

        #endregion

        #region Student

        var studentPermissions = permissions
            .Where(permission =>
                permission.PermissionName == Permissions.UsersRead ||
                permission.PermissionName == Permissions.AuthView)
            .Select(permission =>
                RolePermission.Create(
                    studentRole.Id,
                    permission.PermissionId))
            .ToList();

        context.RolePermissions.AddRange(studentPermissions);

        #endregion

        #region Advisor

        var advisorPermissions = permissions
            .Where(permission =>
                permission.PermissionName == Permissions.UsersRead ||
                permission.PermissionName == Permissions.AuthView)
            .Select(permission =>
                RolePermission.Create(
                    advisorRole.Id,
                    permission.PermissionId))
            .ToList();

        context.RolePermissions.AddRange(advisorPermissions);

        #endregion

        await context.SaveChangesAsync();
    }
}