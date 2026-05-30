namespace EduAdvisor.Application.Commands.AuthModules
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator(IStringLocalizer localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer["Emailrequired"])
                .EmailAddress().WithMessage(localizer["Invalidemail"]);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer["Passwordrequired"]);
        }
    }
}