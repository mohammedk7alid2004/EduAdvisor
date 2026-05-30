using EduAdvisor.Domain.Base;
using EduAdvisor.Domain.Entities.AuthModule;

namespace EduAdvisor.Domain.Entities.Base;

public abstract class BaseEntity : IAuditableEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public string? CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public string? DeletedById { get; protected set; }

    public User? CreatedBy { get; set; }
    public User? UpdatedBy { get; set; }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void SoftDelete(string? deletedById = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedById = deletedById;
    }

    public virtual void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedById = null;
    }
}