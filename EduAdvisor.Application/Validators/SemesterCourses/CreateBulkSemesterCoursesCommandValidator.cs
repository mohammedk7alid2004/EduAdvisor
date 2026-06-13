using EduAdvisor.Application.Commands.SemesterCourses;

namespace EduAdvisor.Application.Validators.SemesterCourses
{
    public sealed class CreateBulkSemesterCoursesCommandValidator
     : AbstractValidator<CreateBulkSemesterCoursesCommand>
    {
        public CreateBulkSemesterCoursesCommandValidator()
        {
            RuleFor(x => x.SemesterId)
                .NotEmpty().WithMessage("Semester ID is required.");

            RuleFor(x => x.CourseAcademicPlanIds)
                .NotEmpty().WithMessage("At least one Course Academic Plan ID is required.")
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("Duplicate Course Academic Plan IDs are not allowed.");
        }
    }


}
