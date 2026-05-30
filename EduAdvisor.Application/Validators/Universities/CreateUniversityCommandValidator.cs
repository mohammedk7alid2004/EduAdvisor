using EduAdvisor.Application.Commands.Universities;

namespace EduAdvisor.Application.Validators.Universities;

public sealed class CreateUniversityCommandValidator
    : AbstractValidator<CreateUniversityCommand>
{
    public CreateUniversityCommandValidator()
    {
        #region Name

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        #endregion

        #region Email

        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email must not exceed 150 characters.");
        });

        #endregion

        #region Website

        When(x => !string.IsNullOrWhiteSpace(x.Website), () =>
        {
            RuleFor(x => x.Website)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Invalid website URL.")
                .MaximumLength(300).WithMessage("Website must not exceed 300 characters.");
        });

        #endregion

        #region PhoneNumber

        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");
        });

        #endregion
    }
}
