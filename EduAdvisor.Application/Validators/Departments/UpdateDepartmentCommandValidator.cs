using EduAdvisor.Application.Commands.Departments;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Validators.Departments;

public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator(IStringLocalizer localizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(localizer["IdIsRequired"]);

        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage(localizer["NameMaxLength"])
            .When(x => x.Name is not null);

        RuleFor(x => x.Code)
            .MaximumLength(20).WithMessage(localizer["CodeMaxLength"])
            .When(x => x.Code is not null);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage(localizer["DescriptionMaxLength"])
            .When(x => x.Description is not null);
    }
}