using EduAdvisor.Application.Commands.CourseModules;
using FluentValidation;

namespace EduAdvisor.Application.Validators.CourseModules;

public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.CourseCode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.CourseName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.CreditHours)
            .InclusiveBetween(1, 12);

        RuleFor(x => x.StandardLevel)
            .InclusiveBetween(1, 4);

        RuleFor(x => x.StandardSemester)
            .InclusiveBetween(1, 8);

        RuleForEach(x => x.PrerequisiteCourseIds)
            .NotEmpty();

        RuleFor(x => x.PrerequisiteCourseIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate prerequisite courses are not allowed.");
    }
}