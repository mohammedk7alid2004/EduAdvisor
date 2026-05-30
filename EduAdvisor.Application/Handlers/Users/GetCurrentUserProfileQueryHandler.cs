using EduAdvisor.Application.DTO.User;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace EduAdvisor.Application.Handlers.Users
{
    public class GetCurrentUserProfileQueryHandler(
       IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IStringLocalizer localizer
    ) : IRequestHandler<GetCurrentUserProfileQuery, Result<CurrentUserResponseDTO>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IStringLocalizer _localizer = localizer;

        public async Task<Result<CurrentUserResponseDTO>> Handle(GetCurrentUserProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Result<CurrentUserResponseDTO>.Failure(_localizer["Unauthorized"], 401);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Result<CurrentUserResponseDTO>.Failure(_localizer["UserNotFound"], 404);

            var dto = new CurrentUserResponseDTO(
                user.Id,
                user.FullName,
                user.Email!,
                user.PhoneNumber!,
                user.EmailConfirmed,
                user.CreatedAt
            );

            return Result<CurrentUserResponseDTO>.Success(
                dto,
                _localizer["Operationcompletedsuccessfully"],
                200
            );
        }
    }
}