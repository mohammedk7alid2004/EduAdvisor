using EduAdvisor.Application.Commands.Faculties;

namespace EduAdvisor.Application.Validators.Faculties;

public sealed class CreateFacultyCommandValidator
    : AbstractValidator<CreateFacultyCommand>
{
    public CreateFacultyCommandValidator()
    {
        #region UniversityId

        RuleFor(x => x.UniversityId)
            .NotEmpty().WithMessage("University is required.")
            .Must(id => id != Guid.Empty).WithMessage("Invalid university ID.");

        #endregion

        #region Name

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        #endregion

        #region Abbreviation

        When(x => !string.IsNullOrWhiteSpace(x.Abbreviation), () =>
        {
            RuleFor(x => x.Abbreviation)
                .MaximumLength(20).WithMessage("Abbreviation must not exceed 20 characters.");
        });

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
    }
}