using EduAdvisor.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EduAdvisor.Infrastructure.Repositories
{
    public class GetCurrentUserRepository : IGetCurrentUserRepository
    {
        private bool _isAuthenticated;
        private string userId;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCurrentUserRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetCurrentUser()
        {
            userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _isAuthenticated = true;
                return $"cart:user:{userId}";
            }
            if (!_httpContextAccessor!.HttpContext.Request.Cookies.TryGetValue("guestId", out var guestId))
            {
                guestId = Guid.NewGuid().ToString();
                _httpContextAccessor.HttpContext.Response.Cookies.Append("guestId", guestId);
                _isAuthenticated = false;
            }

            return $"cart:guest:{guestId}";
        }
        public bool IsAuthenticated()
        {
            return _isAuthenticated;
        }
        public string GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                   ?? user?.FindFirst("sub")?.Value
                   ?? user?.FindFirst("uid")?.Value;
        }
    }
}
