using EduAdvisor.Application.Commands.SemesterCourses;

namespace EduAdvisor.Application.Validators.SemesterCourses
{
    public sealed class UpdateSemesterCourseCommandValidator
    : AbstractValidator<UpdateSemesterCourseCommand>
    {
        public UpdateSemesterCourseCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID is required.");

            RuleFor(x => x.SemesterId)
                .NotEmpty().WithMessage("Semester ID is required.");

            RuleFor(x => x.CourseAcademicPlanId)
                .NotEmpty().WithMessage("Course Academic Plan ID is required.");
        }
    }
}
