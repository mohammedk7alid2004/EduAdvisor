using EduAdvisor.Application.Commands.SemesterCourses;
using FluentValidation;

namespace EduAdvisor.Application.Validators.SemesterCourses;

public sealed class CreateSemesterCourseCommandValidator
    : AbstractValidator<CreateSemesterCourseCommand>
{
    public CreateSemesterCourseCommandValidator()
    {
        RuleFor(x => x.SemesterId)
            .NotEmpty().WithMessage("Semester ID is required.");

        RuleFor(x => x.CourseAcademicPlanId)
            .NotEmpty().WithMessage("Course Academic Plan ID is required.");
    }
}