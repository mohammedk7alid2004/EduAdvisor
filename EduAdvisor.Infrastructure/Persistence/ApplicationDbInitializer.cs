using EduAdvisor.Infrastructure.Data.Seeder.IdentitySeed;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EduAdvisor.Infrastructure.Persistence;

public static class ApplicationDbInitializer
{
    public static async Task InitializeAsync(this WebApplication app)
    {
        var config = app.Configuration;

        var skip = config.GetValue<bool>("Database:SkipMigrationsOnStartup")
            || string.Equals(
                Environment.GetEnvironmentVariable("SKIP_MIGRATIONS_ON_STARTUP"),
                "true",
                StringComparison.OrdinalIgnoreCase);

        if (skip)
        {
            app.Logger.LogWarning(
                "Database migration and seeding skipped.");
            return;
        }

        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var db = services.GetRequiredService<ApplicationDbContext>();

            await db.Database.MigrateAsync();

            await IdentitySeeder.SeedAsync(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILoggerFactory>()
                                 .CreateLogger("DbInitializer");

            logger.LogError(ex, "Error during DB migration and seeding.");
            throw;
        }
    }
}