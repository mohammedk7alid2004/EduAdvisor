using EduAdvisor.Application.Commands.Departments;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Validators.Departments;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator(IStringLocalizer localizer)
    {
        RuleFor(x => x.FacultyId)
            .NotEmpty().WithMessage(localizer["FacultyIsRequired"]);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(localizer["NameIsRequired"])
            .MaximumLength(200).WithMessage(localizer["NameMaxLength"]);

        RuleFor(x => x.Code)
            .MaximumLength(20).WithMessage(localizer["CodeMaxLength"])
            .When(x => x.Code is not null);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage(localizer["DescriptionMaxLength"])
            .When(x => x.Description is not null);
    }
}