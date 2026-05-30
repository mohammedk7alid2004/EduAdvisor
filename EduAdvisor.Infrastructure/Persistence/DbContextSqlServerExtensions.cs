using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Infrastructure.Persistence;
public static class DbContextSqlServerExtensions
{
    public static DbContextOptionsBuilder UseSqlServerWithRetry(
        this DbContextOptionsBuilder optionsBuilder,
        string? connectionString) =>
        optionsBuilder.UseSqlServer(connectionString, sql =>
            sql.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null));
}
