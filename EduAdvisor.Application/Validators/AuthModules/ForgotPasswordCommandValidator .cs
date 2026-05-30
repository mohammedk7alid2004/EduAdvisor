using EduAdvisor.Application.Common.Abstractions.Consts;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator(IStringLocalizer localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizer["EmailRequired"])
                .Matches(RegexPatterns.Email)
                .WithMessage(localizer["InvalidEmail"]);
        }
    }
}