using Microsoft.AspNetCore.Identity;

namespace EduAdvisor.Domain.Entities.RoleModule;

public class ApplicationRole : IdentityRole
{
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}