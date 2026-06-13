using EduAdvisor.Application.Interfaces;
using EduAdvisor.Domain.Base;
using EduAdvisor.Domain.Entities.AcademicModule;
using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Departments;
using EduAdvisor.Domain.Entities.Enrollments;
using EduAdvisor.Domain.Entities.Faculties;
using EduAdvisor.Domain.Entities.RoleModule;
using EduAdvisor.Domain.Entities.Semesters;
using EduAdvisor.Domain.Entities.Universities;
using EduAdvisor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EduAdvisor.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User>, IApplicationDbContext
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly TimeZoneInfo _appTimeZone;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
        : base(options)
    {
        _httpContext = httpContextAccessor;
        _appTimeZone = ResolveAppTimeZone(configuration);
    }

    #region DbSets - Auth & Users

    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students { get; set; }
    public DbSet<Advisor> Advisors { get; set; }

    #endregion

    #region DbSets - Core

    public DbSet<University> Universities { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Department> Departments { get; set; }

    #endregion

    #region DbSets - Subjects & Semesters
    public DbSet<Course> Courses { get; set; }
   public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }

    public DbSet<Semester> Semesters { get; set; }
    public DbSet<CourseAcademicPlan> CourseAcademicPlans { get; set; }


    #endregion

    #region DbSets - Enrollment

    public DbSet<Enrollment> Enrollments { get; set; }

    #endregion

    #region OnModelCreating

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (clrType is null) continue;

            if (!typeof(IAuditableEntity).IsAssignableFrom(clrType)
                || clrType == typeof(User)
                || clrType == typeof(BaseEntity))
                continue;

            modelBuilder.Entity(clrType)
                .HasOne(typeof(User), "CreatedBy")
                .WithMany()
                .HasForeignKey("CreatedById")
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity(clrType)
                .HasOne(typeof(User), "UpdatedBy")
                .WithMany()
                .HasForeignKey("UpdatedById")
                .OnDelete(DeleteBehavior.NoAction);
        }

        modelBuilder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    #endregion

    #region SaveChangesAsync

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();
        var currentUserId = _httpContext.HttpContext?.User?.GetUserId();
        var isAuthenticated = _httpContext.HttpContext?.User?.Identity?.IsAuthenticated == true;
        var now = GetAppNow();

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(x => x.CreatedAt).CurrentValue = now;

                if (isAuthenticated && currentUserId is not null)
                    entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(x => x.CreatedById).IsModified = false;
                entityEntry.Property(x => x.CreatedAt).IsModified = false;
                entityEntry.Property(x => x.UpdatedAt).CurrentValue = now;

                if (isAuthenticated && currentUserId is not null)
                    entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Helpers

    private static TimeZoneInfo ResolveAppTimeZone(IConfiguration configuration)
    {
        var tzId =
            configuration["App:TimeZoneId"]
            ?? configuration["TimeZoneId"]
            ?? Environment.GetEnvironmentVariable("APP_TIME_ZONE_ID")
            ?? Environment.GetEnvironmentVariable("TZ")
            ?? "Egypt Standard Time";

        try { return TimeZoneInfo.FindSystemTimeZoneById(tzId); }
        catch { return TimeZoneInfo.Local; }
    }

    private DateTime GetAppNow() =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _appTimeZone);

    #endregion
}