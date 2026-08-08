using EduAdvisor.Application.Commands.AuthModules;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Validators.AuthModules;

public sealed class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator(IStringLocalizer localizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(localizer["RoleRequired"])
            .MaximumLength(100)
            .WithMessage(localizer["RoleMaxLength"]);
    }
}