using EduAdvisor.Application.Commands.Faculties;

namespace EduAdvisor.Application.Validators.Faculties;

public sealed class DeleteFacultyCommandValidator
 : AbstractValidator<DeleteFacultyCommand>
{
    public DeleteFacultyCommandValidator()
    {
        #region Id

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Faculty ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Invalid faculty ID.");

        #endregion
    }
}
