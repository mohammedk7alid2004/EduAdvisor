using EduAdvisor.Application.Common.Abstractions.Consts;

namespace EduAdvisor.Application.Validators.AuthModules
{
    public class ConfirmEmailCommandValidator:AbstractValidator<ConfirmEmailCommand>    
    {
        public ConfirmEmailCommandValidator(IStringLocalizer localizer)
        {

            RuleFor(x => x.Email)
             .NotEmpty()
             .WithMessage(localizer["EmailRequired"])
             .Matches(RegexPatterns.Email)
             .WithMessage(localizer["InvalidEmail"]);

            RuleFor(x => x.OTP)
                .NotEmpty()
                .Length(6)
                .WithMessage(localizer["OTPMustBe6Digits"]);


        }

    }
}
