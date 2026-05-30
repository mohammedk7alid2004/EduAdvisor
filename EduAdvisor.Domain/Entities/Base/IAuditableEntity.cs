namespace EduAdvisor.Domain.Base;

public interface IAuditableEntity
{
    string? CreatedById { get; set; }
    DateTime CreatedAt { get; set; }

    string? UpdatedById { get; set; }
    DateTime? UpdatedAt { get; set; }
}
