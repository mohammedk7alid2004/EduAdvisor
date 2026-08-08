using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Domain.Entities.RoleModule;
using EduAdvisor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EduAdvisor.Infrastructure.Data.Seeder.IdentitySeed;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<RoleManager<ApplicationRole>>();

        var userManager =
            services.GetRequiredService<UserManager<User>>();

        var context =
            services.GetRequiredService<ApplicationDbContext>();

        await RoleSeed.SeedAsync(roleManager);

        await PermissionSeed.SeedAsync(context);

        await RolePermissionSeed.SeedAsync(
            context,
            roleManager);

        await UserSeed.SeedAsync(userManager);
    }
}