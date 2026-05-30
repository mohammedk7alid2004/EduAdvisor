using EduAdvisor.Application.Commands.AuthModules;
using Microsoft.AspNetCore.Identity;


namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class CreateRoleCommandHandler(
        RoleManager<IdentityRole> roleManager,
        IStringLocalizer localizer
    ) : IRequestHandler<CreateRoleCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (await roleManager.RoleExistsAsync(request.name))
                return Result<string>.Failure(localizer["RoleAlreadyExists"], 400);

            var result = await roleManager.CreateAsync(new IdentityRole(request.name));

            if (!result.Succeeded)
                return Result<string>.Failure(localizer["Operationfailed"], 400);

            return Result<string>.Success("Created", localizer["Operationcompletedsuccessfully"], 201);
        }
    }
}
