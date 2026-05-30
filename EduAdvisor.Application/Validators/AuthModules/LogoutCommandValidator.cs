namespace EduAdvisor.Application.Commands.AuthModules
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator(IStringLocalizer localizer)
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage(localizer["Refreshtokenrequired"]);
        }
    }
}