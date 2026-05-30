using EduAdvisor.Application.Commands.Universities;

namespace EduAdvisor.Application.Validators.Universities;

public sealed class UpdateUniversityCommandValidator
    : AbstractValidator<UpdateUniversityCommand>
{
    public UpdateUniversityCommandValidator()
    {
        #region Id

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("University ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Invalid university ID.");

        #endregion

        #region Name (if sent)

        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        });

        #endregion

        #region Email (if sent)

        When(x => x.Email is not null, () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email must not exceed 150 characters.");
        });

        #endregion

        #region Website (if sent)

        When(x => x.Website is not null, () =>
        {
            RuleFor(x => x.Website)
                .Must(url => string.IsNullOrWhiteSpace(url) ||
                             Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Invalid website URL.")
                .MaximumLength(300).WithMessage("Website must not exceed 300 characters.");
        });

        #endregion

        #region PhoneNumber (if sent)

        When(x => x.PhoneNumber is not null, () =>
        {
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");
        });

        #endregion
    }
}
