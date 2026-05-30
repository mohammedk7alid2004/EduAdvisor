using EduAdvisor.Application.Common.Abstractions.Consts;

namespace EduAdvisor.Application.Validators.AuthModules
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator(IStringLocalizer localizer)
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(localizer["Currentpasswordrequired"]);

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