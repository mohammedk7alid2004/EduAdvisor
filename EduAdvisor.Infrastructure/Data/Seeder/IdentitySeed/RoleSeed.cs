using EduAdvisor.Domain.Entities.RoleModule;
using Microsoft.AspNetCore.Identity;

namespace EduAdvisor.Infrastructure.Data.Seeder.IdentitySeed;

public static class RoleSeed
{
    public static async Task SeedAsync(
        RoleManager<ApplicationRole> roleManager)
    {
        string[] roles =
        [
            "Admin",
            "Student",
            "Advisor"
        ];

        foreach (var roleName in roles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var role = new ApplicationRole
            {
                Name = roleName
            };

            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}': {errors}");
            }
        }
    }
}