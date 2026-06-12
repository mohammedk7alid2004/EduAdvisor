using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Departments;
using EduAdvisor.Domain.Entities.Universities;

namespace EduAdvisor.Domain.Entities.Faculties;

public sealed class Faculty : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Website { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? LogoUrl { get; private set; }
    public string? Email { get; private set; }
    public string? Abbreviation { get; private set; }
    public Guid UniversityId { get; private set; }

    public University University { get; private set; } = default!;

    private readonly List<Department> _departments = [];
    public IReadOnlyCollection<Department> Departments => _departments;



    private Faculty() { }

    public Faculty(string name, Guid universityId, string? abbreviation = null, string? email = null, string? website = null, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (universityId == Guid.Empty)
            throw new ArgumentException("UniversityId is required.", nameof(universityId));

        Name = name.Trim();
        UniversityId = universityId;
        Abbreviation = abbreviation?.Trim();
        Email = email?.Trim();
        Website = website?.Trim();
        Description = description?.Trim();
        IsActive = true;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Name = name.Trim();
        UpdateTimestamp();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        UpdateTimestamp();
    }

    public void UpdateWebsite(string? website)
    {
        Website = website?.Trim();
        UpdateTimestamp();
    }

    public void UpdateEmail(string? email)
    {
        Email = email?.Trim();
        UpdateTimestamp();
    }

    public void SetLogoUrl(string? logoUrl)
    {
        LogoUrl = logoUrl?.Trim();
        UpdateTimestamp();
    }

    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }
}