// Validator
using EduAdvisor.Application.Commands.CourseModules;
using FluentValidation;

namespace EduAdvisor.Application.Validators.CourseModules;

public sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");

        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course name is required.")
            .MaximumLength(200).WithMessage("Course name must not exceed 200 characters.");

        RuleFor(x => x.CreditHours)
            .GreaterThan(0).WithMessage("Credit hours must be greater than zero.")
            .LessThanOrEqualTo(6).WithMessage("Credit hours must not exceed 6.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid course type.");

        RuleFor(x => x.StandardLevel)
            .InclusiveBetween(1, 8).WithMessage("Standard level must be between 1 and 8.");

        RuleFor(x => x.StandardSemester)
            .InclusiveBetween(1, 2).WithMessage("Standard semester must be between 1 and 2.");

        RuleFor(x => x.PrerequisiteCourseIds)
            .NotNull().WithMessage("Prerequisite course IDs must not be null.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Prerequisite course IDs must not contain duplicates.");
    }
}