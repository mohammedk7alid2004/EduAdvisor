using EduAdvisor.Application.Commands.CourseAcademicPlans;

namespace EduAdvisor.Application.Validators.CourseAcademicPlans;

public sealed class CreateCourseAcademicPlanCommandValidator
    : AbstractValidator<CreateCourseAcademicPlanCommand>
{
    public CreateCourseAcademicPlanCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");

        RuleFor(x => x.Level)
            .InclusiveBetween(1, 4).WithMessage("Level must be between 1 and 4.");

        RuleFor(x => x.StandardSemester)
            .InclusiveBetween(1, 2).WithMessage("Standard semester must be 1 or 2.");
    }
}

