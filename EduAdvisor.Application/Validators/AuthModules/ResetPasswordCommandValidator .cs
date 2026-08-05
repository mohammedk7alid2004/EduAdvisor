using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Common.Abstractions.Consts;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Validators.AuthModules;

public sealed class ResetPasswordCommandValidator
    : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator(
        IStringLocalizer localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(localizer["EmailRequired"])
            .EmailAddress()
            .WithMessage(localizer["InvalidEmail"]);

        RuleFor(x => x.Otp)
            .NotEmpty()
            .WithMessage(localizer["OtpNotProvided"])
            .Length(6)
            .WithMessage(localizer["InvalidOtp"]);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(localizer["PasswordRequired"])
            .Matches(RegexPatterns.Password)
            .WithMessage(localizer["WeakPassword"]);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage(localizer["ConfirmPasswordRequired"])
            .Equal(x => x.NewPassword)
            .WithMessage(localizer["PasswordsNotMatch"]);
    }
}