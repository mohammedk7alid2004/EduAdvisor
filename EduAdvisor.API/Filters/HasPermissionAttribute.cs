using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class HasPermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _permissionName;

        public HasPermissionAttribute(string permissionName)
        {
            _permissionName = permissionName; 
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var authService = context.HttpContext.RequestServices
                .GetRequiredService<IAuthorizationService>();

            var requirement = new PermissionRequirement(_permissionName);

            var result = await authService.AuthorizeAsync(
                context.HttpContext.User,
                null,
                requirement);

            if (!result.Succeeded)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}