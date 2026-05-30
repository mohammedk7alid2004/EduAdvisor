using EduAdvisor.Application.Common.Abstractions.Consts;

namespace EduAdvisor.Application.Validators.AuthModules
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator(IStringLocalizer localizer)
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage(localizer["Tokennotprovided"]);

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
}