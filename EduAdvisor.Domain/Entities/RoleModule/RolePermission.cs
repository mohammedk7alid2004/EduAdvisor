namespace EduAdvisor.Domain.Entities.RoleModule;

public class RolePermission
{
    public Guid RolePermissionId { get; set; }

    public string RoleId { get; set; } = string.Empty;

    public Guid PermissionId { get; set; }

    public ApplicationRole Role { get; set; } = default!;

    public Permission Permission { get; set; } = default!;

    protected RolePermission()
    {
    }

    private RolePermission(
        string roleId,
        Guid permissionId)
    {
        RolePermissionId = Guid.NewGuid();
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public static RolePermission Create(
        string roleId,
        Guid permissionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(roleId);

        if (permissionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Permission ID cannot be empty.",
                nameof(permissionId));
        }

        return new RolePermission(
            roleId,
            permissionId);
    }
}