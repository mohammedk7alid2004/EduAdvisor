using System.Text.RegularExpressions;
using EduAdvisor.Application.Common.Abstractions.Consts;

namespace EduAdvisor.Application.Validators.AuthModules;

public class RegisterAdvisorCommandValidator : AbstractValidator<RegisterAdvisorCommand>
{
    public RegisterAdvisorCommandValidator(IStringLocalizer localizer)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(localizer["FirstNameIsRequired"])
            .MaximumLength(50).WithMessage(localizer["FirstNameMaxLength"]);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(localizer["LastNameIsRequired"])
            .MaximumLength(50).WithMessage(localizer["LastNameMaxLength"]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["EmailIsRequired"])
            .Matches(RegexPatterns.UniversityEmail).WithMessage(localizer["InvalidUniversityEmail"]);

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage(localizer["DepartmentIsRequired"]);

        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage(localizer["NationalIdIsRequired"]);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage(localizer["PhoneIsRequired"]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer["PasswordIsRequired"])
            .Matches(RegexPatterns.Password).WithMessage(localizer["PasswordInvalid"]);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage(localizer["PasswordsDoNotMatch"]);

        RuleFor(x => x.ProfileImage)
            .Must(file => file == null || file.Length > 0)
            .WithMessage(localizer["InvalidImageFile"])
            .Must(file => file == null ||
                Regex.IsMatch(file.FileName, RegexPatterns.ImageFilePattern))
            .WithMessage(localizer["InvalidImageFormat"])
            .When(x => x.ProfileImage is not null);
    }
}