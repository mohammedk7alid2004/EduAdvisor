using EduAdvisor.Application.Common.Abstractions.Consts;
using EduAdvisor.Domain.Entities.RoleModule;
using EduAdvisor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace EduAdvisor.Infrastructure.Data.Seeder.IdentitySeed;

public static class RolePermissionSeed
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager)
    {
        if (context.RolePermissions.Any()) return;

        var adminRole = await roleManager.FindByNameAsync("Admin");
        var studentRole = await roleManager.FindByNameAsync("Student");
        var advisorRole = await roleManager.FindByNameAsync("Advisor");

        var permissions = context.Permissions.ToList();

        #region Admin — كل الصلاحيات

        foreach (var perm in permissions)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = adminRole!.Id,
                PermissionId = perm.PermissionId
            });
        }

        #endregion

        #region Student — صلاحيات محدودة

        var studentPerms = permissions.Where(p =>
            p.PermissionName == Permissions.UsersRead ||
            p.PermissionName == Permissions.AuthView);

        foreach (var perm in studentPerms)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = studentRole!.Id,
                PermissionId = perm.PermissionId
            });
        }

        #endregion

        #region Advisor — صلاحيات متوسطة

        var advisorPerms = permissions.Where(p =>
            p.PermissionName == Permissions.UsersRead ||
            p.PermissionName == Permissions.AuthView);

        foreach (var perm in advisorPerms)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = advisorRole!.Id,
                PermissionId = perm.PermissionId
            });
        }

        #endregion

        await context.SaveChangesAsync();
    }
}