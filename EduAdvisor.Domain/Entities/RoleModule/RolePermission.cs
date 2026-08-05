
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EduAdvisor.Domain.Entities.RoleModule;

public class RolePermission
{
    [Key]
    public Guid RolePermissionId { get; set; }
    public string RoleId { get; set; } =string.Empty;
    public Guid PermissionId { get; set; }
    public ApplicationRole Role { get; set; } = default!;
    public Permission Permission { get; set; }=default!;
}
