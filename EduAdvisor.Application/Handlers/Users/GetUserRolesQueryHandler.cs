using EduAdvisor.Application.Queries.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduAdvisor.Application.Handlers.Users
{
    public class GetUserRolesQueryHandler(
      UserManager<User> userManager,
      IStringLocalizer localizer
  ) : IRequestHandler<GetUserRolesQuery, Result<List<string>>>
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IStringLocalizer _localizer = localizer;

        public async Task<Result<List<string>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result<List<string>>.Failure(_localizer["UserNotFound"], 404);

            var roles = await _userManager.GetRolesAsync(user);

            return Result<List<string>>.Success(roles.ToList(), _localizer["Operationcompletedsuccessfully"], 200);
        }
    }
}
