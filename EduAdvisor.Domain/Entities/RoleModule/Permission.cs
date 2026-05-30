
using System.ComponentModel.DataAnnotations;

namespace EduAdvisor.Domain.Entities.RoleModule;

public class Permission
{
    [Key]
    public Guid PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public ICollection<RolePermission> RolePermissions { get; set; }= new List<RolePermission>();
}
