using Microsoft.AspNetCore.Identity;

namespace EduAdvisor.Infrastructure.Data.Seeder.IdentitySeed;

public static class RoleSeed
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Student", "Advisor" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}