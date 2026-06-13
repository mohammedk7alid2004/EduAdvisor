using System.Text.RegularExpressions;
using EduAdvisor.Application.Common.Abstractions.Consts;

namespace EduAdvisor.Application.Validators.AuthModules;

public class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
{
    public RegisterStudentCommandValidator(IStringLocalizer localizer)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(localizer["FirstNameIsRequired"])
            .MaximumLength(50).WithMessage(localizer["FirstNameMaxLength"]);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(localizer["LastNameIsRequired"])
            .MaximumLength(50).WithMessage(localizer["LastNameMaxLength"]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["EmailIsRequired"])
            .Matches(RegexPatterns.UniversityEmail)
            .WithMessage(localizer["InvalidUniversityEmail"]);

        RuleFor(x => x.StudentCode)
            .NotEmpty()
            .WithMessage(localizer["StudentCodeIsRequired"])
            .MaximumLength(20)
            .WithMessage(localizer["StudentCodeMaxLength"]);

        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithMessage(localizer["DepartmentIsRequired"])
            .NotEqual(Guid.Empty)
            .WithMessage(localizer["InvalidDepartment"]);

        RuleFor(x => x.NationalId)
     .NotEmpty()
     .WithMessage(localizer["NationalIdIsRequired"])
     .Matches(@"^\d{14}$")
     .WithMessage(localizer["NationalIdMustBe14Digits"]);
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(localizer["PasswordIsRequired"])
            .Matches(RegexPatterns.Password)
            .WithMessage(localizer["PasswordInvalid"]);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage(localizer["PasswordsDoNotMatch"]);

        RuleFor(x => x.ProfileImage)
            .Must(file => file == null || file.Length > 0)
            .WithMessage(localizer["InvalidImageFile"])
            .Must(file =>
                file == null ||
                Regex.IsMatch(file.FileName, RegexPatterns.ImageFilePattern))
            .WithMessage(localizer["InvalidImageFormat"])
            .When(x => x.ProfileImage is not null);
    }
}