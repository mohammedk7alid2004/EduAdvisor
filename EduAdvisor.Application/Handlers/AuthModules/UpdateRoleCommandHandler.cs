using Microsoft.AspNetCore.Identity;

namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class UpdateRoleCommandHandler(
        RoleManager<IdentityRole> roleManager,
        IStringLocalizer localizer
    ) : IRequestHandler<UpdateRoleCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await roleManager.FindByIdAsync(request.Id);

            if (role == null)
                return Result<string>.Failure(localizer["RoleNotFound"], 404);

            role.Name = request.Name;

            var result = await roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                return Result<string>.Failure(localizer["Operationfailed"], 400);

            return Result<string>.Success("Updated", localizer["Operationcompletedsuccessfully"], 200);
        }
    }
}
