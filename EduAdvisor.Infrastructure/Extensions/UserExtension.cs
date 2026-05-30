using System.Security.Claims;

namespace EduAdvisor.Infrastructure.Extensions
{
    public static class UserExtension
    {
        public static string? GetUserId(this ClaimsPrincipal? user)
        {
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? user?.FindFirst("sub")?.Value
                   ?? user?.FindFirst("uid")?.Value;
        }
    }
}
