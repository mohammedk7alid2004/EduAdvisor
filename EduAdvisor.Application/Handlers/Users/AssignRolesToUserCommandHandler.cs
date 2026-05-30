using EduAdvisor.Application.Commands.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
namespace EduAdvisor.Application.Handlers.Users
{
        public class AssignRolesToUserCommandHandler(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IStringLocalizer localizer
        ) : IRequestHandler<AssignRolesToUserCommand, Result<bool>>
        {
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IStringLocalizer _localizer = localizer;

        public async Task<Result<bool>> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                    return Result<bool>.Failure(localizer["UserNotFound"], 404);

                foreach (var role in request.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                        return Result<bool>.Failure(localizer["RoleNotFound"], 404);
                }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = request.Roles
                .Except(currentRoles)
                .ToList();

            if (rolesToAdd.Any())
            {
                var result = await _userManager.AddToRolesAsync(user, rolesToAdd);

                if (!result.Succeeded)
                {
                    var error = result.Errors.Select(e => e.Description).First();
                    return Result<bool>.Failure(error, 400);
                }
            }

            return Result<bool>.Success(true,localizer["Rolesassignedsuccessfully"], 200);
            }
        }
    }
