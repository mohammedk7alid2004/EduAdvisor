using EduAdvisor.Application.Commands.Faculties;

namespace EduAdvisor.Application.Validators.Faculties;

public sealed class UpdateFacultyCommandValidator
    : AbstractValidator<UpdateFacultyCommand>
{
    public UpdateFacultyCommandValidator()
    {
        #region Id

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Faculty ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Invalid faculty ID.");

        #endregion

        #region Name (if sent)

        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        });

        #endregion

        #region Abbreviation (if sent)

        When(x => x.Abbreviation is not null, () =>
        {
            RuleFor(x => x.Abbreviation)
                .MaximumLength(20).WithMessage("Abbreviation must not exceed 20 characters.");
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
    }
}
