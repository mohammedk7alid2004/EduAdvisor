namespace EduAdvisor.Application.Validators.AuthModules
{
    public class RefreshTokenCommandValidator:AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator(IStringLocalizer localizer)
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage(localizer["Refreshtokenrequired"]);
        }
    }
}
