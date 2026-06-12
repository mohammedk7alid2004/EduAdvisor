using EduAdvisor.Application.Commands.Users;
using FluentValidation;

namespace EduAdvisor.Application.Validators.Users;

public sealed class AssignStudentsToAdvisorCommandValidator
    : AbstractValidator<AssignStudentsToAdvisorCommand>
{
    public AssignStudentsToAdvisorCommandValidator()
    {
        #region AdvisorId

        RuleFor(x => x.AdvisorId)
            .NotEmpty().WithMessage("Advisor ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Invalid advisor ID.");

        #endregion

        #region StudentIds

        RuleFor(x => x.StudentIds)
            .NotEmpty().WithMessage("At least one student ID is required.");

        RuleFor(x => x.StudentIds)
            .Must(ids => ids != null && ids.All(id => id != Guid.Empty))
            .WithMessage("One or more student IDs are invalid.")
            .When(x => x.StudentIds is { Count: > 0 });

        RuleFor(x => x.StudentIds)
            .Must(ids => ids != null && ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate student IDs are not allowed.")
            .When(x => x.StudentIds is { Count: > 0 });

        #endregion
    }
}