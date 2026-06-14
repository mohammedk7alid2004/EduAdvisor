using EduAdvisor.Application.Commands.Student;

namespace EduAdvisor.Application.Validators.Student
{
    public sealed class SubmitRegistrationRequestCommandValidator
     : AbstractValidator<SubmitRegistrationRequestCommand>
    {
        public SubmitRegistrationRequestCommandValidator()
        {
            RuleFor(x => x.SemesterCourseIds)
                .NotEmpty()
                .WithMessage("At least one course must be selected.");

            RuleFor(x => x.SemesterCourseIds)
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("Duplicate courses are not allowed.");
        }
    }
}

