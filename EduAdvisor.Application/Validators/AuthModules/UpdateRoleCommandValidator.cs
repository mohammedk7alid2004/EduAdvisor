namespace EduAdvisor.Application.Validators.AuthModules
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator(IStringLocalizer localizer)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer["Rolerequired"]).MinimumLength(3);
        }
    }
}
