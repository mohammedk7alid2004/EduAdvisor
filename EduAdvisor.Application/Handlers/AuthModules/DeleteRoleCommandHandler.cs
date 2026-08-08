using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Domain.Entities.RoleModule;
using Microsoft.AspNetCore.Identity;


namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class DeleteRoleCommandHandler(
        RoleManager<ApplicationRole> roleManager,
        IStringLocalizer localizer
    ) : IRequestHandler<DeleteRoleCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await roleManager.FindByIdAsync(request.Id);

            if (role == null)
                return Result<string>.Failure(localizer["RoleNotFound"], 404);

            var result = await roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                return Result<string>.Failure(localizer["Operationfailed"], 400);

            return Result<string>.Success("Deleted", localizer["Operationcompletedsuccessfully"], 200);
        }
    }
}
