using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Departments;
using EduAdvisor.Domain.Entities.Enrollments;
using EduAdvisor.Domain.Entities.Faculties;
using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Domain.Entities.Subjects;

public sealed class Subject : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int CreditHours { get; private set; }
    public SubjectType Type { get; private set; }
    public string? Description { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid FacultyId { get; private set; }
    public int? RecommendedLevel { get; private set; }

    private readonly List<SubjectPrerequisite> _prerequisites = [];
    public IReadOnlyCollection<SubjectPrerequisite> Prerequisites => _prerequisites;

    private readonly List<SubjectPrerequisite> _requiredFor = [];
    public IReadOnlyCollection<SubjectPrerequisite> RequiredFor => _requiredFor;

    private readonly List<Enrollment> _enrollments = [];
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;

    public Department Department { get; private set; } = default!;
    public Faculty Faculty { get; private set; } = default!;

    private Subject() { }

    public Subject(
        string code,
        string name,
        int creditHours,
        SubjectType type,
        Guid departmentId,
        Guid facultyId,
        int? recommendedLevel = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (creditHours <= 0)
            throw new ArgumentOutOfRangeException(nameof(creditHours), "Credit hours must be greater than zero.");

        if (recommendedLevel is <= 0)
            throw new ArgumentOutOfRangeException(nameof(recommendedLevel), "Recommended level must be greater than zero.");

        if (departmentId == Guid.Empty)
            throw new ArgumentException("DepartmentId is required.", nameof(departmentId));

        if (facultyId == Guid.Empty)
            throw new ArgumentException("FacultyId is required.", nameof(facultyId));

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        CreditHours = creditHours;
        Type = type;
        DepartmentId = departmentId;
        FacultyId = facultyId;
        RecommendedLevel = recommendedLevel;
        Description = description?.Trim();
    }

    #region Updates

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

    public void UpdateCreditHours(int creditHours)
    {
        if (creditHours <= 0)
            throw new ArgumentOutOfRangeException(nameof(creditHours));

        CreditHours = creditHours;
        UpdateTimestamp();
    }

    public void UpdateRecommendedLevel(int? recommendedLevel)
    {
        if (recommendedLevel is <= 0)
            throw new ArgumentOutOfRangeException(nameof(recommendedLevel));

        RecommendedLevel = recommendedLevel;
        UpdateTimestamp();
    }

    public void UpdateDepartment(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            throw new ArgumentException("DepartmentId is required.", nameof(departmentId));

        DepartmentId = departmentId;
        UpdateTimestamp();
    }

    public void UpdateFaculty(Guid facultyId)
    {
        if (facultyId == Guid.Empty)
            throw new ArgumentException("FacultyId is required.", nameof(facultyId));

        FacultyId = facultyId;
        UpdateTimestamp();
    }

    #endregion

    #region Prerequisites

    public void AddPrerequisite(SubjectPrerequisite prerequisite)
    {
        if (_prerequisites.Any(x => x.PrerequisiteSubjectId == prerequisite.PrerequisiteSubjectId))
            throw new ArgumentException("Prerequisite already exists.");

        _prerequisites.Add(prerequisite);
        UpdateTimestamp();
    }

    public void RemovePrerequisite(Guid prerequisiteSubjectId)
    {
        var prerequisite = _prerequisites
            .FirstOrDefault(x => x.PrerequisiteSubjectId == prerequisiteSubjectId);

        if (prerequisite is null) return;

        _prerequisites.Remove(prerequisite);
        UpdateTimestamp();
    }

    public bool CheckPrerequisites(IEnumerable<Guid> completedSubjectIds)
    {
        if (_prerequisites.Count == 0) return true;

        var completedSet = completedSubjectIds.ToHashSet();
        return _prerequisites.All(x => completedSet.Contains(x.PrerequisiteSubjectId));
    }

    public IEnumerable<Guid> GetMissingPrerequisites(IEnumerable<Guid> completedSubjectIds)
    {
        if (_prerequisites.Count == 0) return Enumerable.Empty<Guid>();

        var completedSet = completedSubjectIds.ToHashSet();
        return _prerequisites
            .Where(x => !completedSet.Contains(x.PrerequisiteSubjectId))
            .Select(x => x.PrerequisiteSubjectId);
    }

    #endregion

    #region Helpers

    public bool IsElective() => Type == SubjectType.Elective;
    public bool IsCompulsory() => Type == SubjectType.Compulsory;

    #endregion
}