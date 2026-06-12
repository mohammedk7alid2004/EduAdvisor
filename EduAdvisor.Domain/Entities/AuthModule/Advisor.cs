using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Departments;

namespace EduAdvisor.Domain.Entities.AuthModule;

public class Advisor : BaseEntity
{
    private readonly List<Student> _students = [];

    public string UserId { get; private set; } = string.Empty;
    public Guid DepartmentId { get; private set; }
    public bool IsPending { get; private set; } = true;

    public virtual User User { get; private set; } = default!;
    public virtual Department Department { get; private set; } = default!;

    public virtual IReadOnlyCollection<Student> Students => _students.AsReadOnly();

    private Advisor() { }

    public Advisor(string userId, Guid departmentId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required.", nameof(userId));

        if (departmentId == Guid.Empty)
            throw new ArgumentException("DepartmentId is required.", nameof(departmentId));

        UserId = userId;
        DepartmentId = departmentId;
        IsPending = true;
    }

    #region Status

    public void Approve()
    {
        IsPending = false;
        UpdateTimestamp();
    }

    public void SetPending()
    {
        IsPending = true;
        UpdateTimestamp();
    }

    #endregion

    #region Department

    public void UpdateDepartment(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            throw new ArgumentException("DepartmentId is required.", nameof(departmentId));

        DepartmentId = departmentId;
        UpdateTimestamp();
    }

    #endregion

    #region Student Management

    public void AddStudent(Student student)
    {
        if (student == null) throw new ArgumentNullException(nameof(student));

        if (!_students.Any(s => s.Id == student.Id))
        {
            _students.Add(student);
            student.AssignAdvisor(Id);
        }
    }

    #endregion
}