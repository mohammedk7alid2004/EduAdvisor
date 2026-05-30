using EduAdvisor.Application.Commands.Departments;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Validators.Departments;

public class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
{
    public DeleteDepartmentCommandValidator(IStringLocalizer localizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(localizer["IdIsRequired"]);
    }
}