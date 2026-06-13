using EduAdvisor.Application.Commands.Semesters;
using FluentValidation;

namespace EduAdvisor.Application.Validators.Semesters;

public sealed class UpdateSemesterCommandValidator : AbstractValidator<UpdateSemesterCommand>
{
    public UpdateSemesterCommandValidator()
    {
        RuleFor(x => x.SemesterId)
            .NotEmpty().WithMessage("Semester ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Semester name is required.")
            .MaximumLength(100).WithMessage("Semester name must not exceed 100 characters.");

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100).WithMessage("Year must be between 2000 and 2100.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

        RuleFor(x => x.StandardSemesterNumber)
            .InclusiveBetween(1, 2).WithMessage("Standard semester number must be 1 or 2.");
    }
}