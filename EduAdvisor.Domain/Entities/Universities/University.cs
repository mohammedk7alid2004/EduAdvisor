using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Faculties;

namespace EduAdvisor.Domain.Entities.Universities;

public sealed class University : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Address { get; private set; }
    public string? Email { get; private set; }
    public string? Website { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Faculty> _faculties = [];
    public IReadOnlyCollection<Faculty> Faculties => _faculties;

    private University() { }

    public University(string name, string? description = null, string? address = null, string? email = null, string? website = null, string? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        Address = address?.Trim();
        Email = email?.Trim();
        Website = website?.Trim();
        PhoneNumber = phoneNumber?.Trim();
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

    public void UpdateContact(string? email, string? phoneNumber, string? website)
    {
        Email = email?.Trim();
        PhoneNumber = phoneNumber?.Trim();
        Website = website?.Trim();
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
    public void UpdateAddress(string? address)
    {
        Address = address?.Trim();
        UpdateTimestamp();
    }
}