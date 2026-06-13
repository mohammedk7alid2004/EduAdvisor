using EduAdvisor.Application.Commands.Departments;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Validators.Departments;

public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator(IStringLocalizer localizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(localizer["IdIsRequired"])
            .NotEqual(Guid.Empty)
            .WithMessage(localizer["InvalidId"]);

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage(localizer["NameMaxLength"])
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Code)
            .MaximumLength(20)
            .WithMessage(localizer["CodeMaxLength"])
            .When(x => !string.IsNullOrWhiteSpace(x.Code));

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage(localizer["DescriptionMaxLength"])
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}