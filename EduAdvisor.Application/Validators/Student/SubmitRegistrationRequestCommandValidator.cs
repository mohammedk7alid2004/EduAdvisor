using EduAdvisor.Application.Commands.Student;
using FluentValidation;

namespace EduAdvisor.Application.Validators.Student;

public sealed class SubmitRegistrationRequestCommandValidator
    : AbstractValidator<SubmitRegistrationRequestCommand>
{
    public SubmitRegistrationRequestCommandValidator()
    {
        RuleFor(command => command.SemesterCourseIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Course selection is required.")
            .NotEmpty()
            .WithMessage("At least one course must be selected.")
            .Must(courseIds =>
                courseIds.Distinct().Count() == courseIds.Count)
            .WithMessage("Duplicate courses are not allowed.");
    }
}