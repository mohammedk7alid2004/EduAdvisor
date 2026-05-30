namespace EduAdvisor.Application.Validators.AuthModules
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleValidator(IStringLocalizer localizer)
        {
            RuleFor(x => x.name)
                .NotEmpty()
              .WithMessage(localizer["Rolerequired"]);
        }
    }
}
