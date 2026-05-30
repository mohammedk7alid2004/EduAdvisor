using EduAdvisor.Domain.Base;
using EduAdvisor.Domain.Entities.RoleModule;
using Microsoft.AspNetCore.Identity;

namespace EduAdvisor.Domain.Entities.AuthModule;

public class User : IdentityUser, IAuditableEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string NationalId { get; private set; } = string.Empty;
    public string? ProfileImageUrl { get; private set; }
    public bool IsDisabled { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedById { get; set; }

    public ICollection<RolePermission> Roles { get; private set; } = new HashSet<RolePermission>();

    protected User() { }

    public User(string fullName, string email, string nationalId)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required.", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        FullName = fullName.Trim();
        Email = email.Trim();
        UserName = email.Trim();
        NationalId = nationalId?.Trim() ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        IsDisabled = false;
    }

    public void UpdateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required.", nameof(fullName));

        FullName = fullName.Trim();
    }

    public void SetNationalId(string nationalId)
        => NationalId = nationalId?.Trim() ?? string.Empty;

    public void SetProfileImage(string? url)
        => ProfileImageUrl = url?.Trim();

    public void Disable() => IsDisabled = true;
    public void Enable() => IsDisabled = false;
}