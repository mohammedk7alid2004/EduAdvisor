using EduAdvisor.Application.Commands.Universities;

namespace EduAdvisor.Application.Validators.Universities;

public sealed class DeleteUniversityCommandValidator
 : AbstractValidator<DeleteUniversityCommand>
{
    public DeleteUniversityCommandValidator()
    {
        #region Id

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("University ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Invalid university ID.");

        #endregion
    }
}
