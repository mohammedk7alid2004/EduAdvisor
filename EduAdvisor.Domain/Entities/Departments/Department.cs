using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Faculties;
using EduAdvisor.Domain.Entities.AcademicModule;

namespace EduAdvisor.Domain.Entities.Departments;

public sealed class Department : BaseEntity
{
    private readonly List<Course> _courses = new();

    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public string? Description { get; private set; }
    public Guid FacultyId { get; private set; }
    public Faculty Faculty { get; private set; } = default!;

    public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();

    private Department() { }

    public Department(string name, Guid facultyId, string? code = null, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (facultyId == Guid.Empty)
            throw new ArgumentException("FacultyId is required.", nameof(facultyId));

        Name = name.Trim();
        FacultyId = facultyId;
        Code = code?.Trim();
        Description = description?.Trim();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        Name = name.Trim();
        UpdateTimestamp();
    }

    public void UpdateCode(string? code)
    {
        Code = code?.Trim();
        UpdateTimestamp();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        UpdateTimestamp();
    }

    public void UpdateFaculty(Guid facultyId)
    {
        if (facultyId == Guid.Empty)
            throw new ArgumentException("FacultyId is required.", nameof(facultyId));
        FacultyId = facultyId;
        UpdateTimestamp();
    }
}