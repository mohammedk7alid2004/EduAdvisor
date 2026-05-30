using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.Subjects;

namespace EduAdvisor.Domain.Entities.Subjects;

public sealed class SubjectPrerequisite : BaseEntity
{
    public Guid SubjectId { get; private set; }
    public Guid PrerequisiteSubjectId { get; private set; }

    public Subject Subject { get; private set; } = default!;
    public Subject PrerequisiteSubject { get; private set; } = default!;

    private SubjectPrerequisite() { }

    public SubjectPrerequisite(Guid subjectId, Guid prerequisiteSubjectId)
    {
        if (subjectId == Guid.Empty)
            throw new ArgumentException("SubjectId is required.", nameof(subjectId));

        if (prerequisiteSubjectId == Guid.Empty)
            throw new ArgumentException("PrerequisiteSubjectId is required.", nameof(prerequisiteSubjectId));

        if (subjectId == prerequisiteSubjectId)
            throw new ArgumentException("A subject cannot be a prerequisite of itself.");

        SubjectId = subjectId;
        PrerequisiteSubjectId = prerequisiteSubjectId;
    }
}