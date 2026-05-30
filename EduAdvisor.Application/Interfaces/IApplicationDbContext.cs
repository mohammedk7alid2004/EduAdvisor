using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Domain.Entities.Departments;
using EduAdvisor.Domain.Entities.Faculties;
using EduAdvisor.Domain.Entities.RoleModule;
using EduAdvisor.Domain.Entities.Universities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Interfaces;

public interface IApplicationDbContext
{
    #region Auth & Users

    DbSet<User> Users { get; }
    DbSet<IdentityRole> Roles { get; }
    DbSet<RolePermission> RolePermissions { get; set; }
    DbSet<Permission> Permissions { get; set; }
    DbSet<IdentityUserRole<string>> UserRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    #endregion

    #region Core

    DbSet<Student> Students { get; set; }
    DbSet<Advisor> Advisors { get; set; }
    DbSet<Department> Departments { get; set; }
    DbSet<University>Universities { get; set; }
    DbSet<Faculty> Faculties { get; set; }
    #endregion

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}