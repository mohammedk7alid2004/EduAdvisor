using EduAdvisor.Domain.Entities.AuthModule;
using Microsoft.AspNetCore.Identity;

namespace EduAdvisor.Infrastructure.Data.Seeder.IdentitySeed;

public static class UserSeed
{
    public static async Task SeedAsync(UserManager<User> userManager)
    {
        var email = "admin@EduAdvisor.com";
        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var admin = new User("Admin", email, "0000000000")
        {
            UserName = "admin",
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(admin, "P@ssword123");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
    }
}