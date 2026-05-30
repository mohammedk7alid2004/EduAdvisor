using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class ChangePasswordCommandHandler(
        UserManager<User> userManager,
        IGetCurrentUserRepository currentUser,
        IApplicationDbContext context,
        IStringLocalizer localizer
    ) : IRequestHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IGetCurrentUserRepository _currentUser = currentUser;
        private readonly IApplicationDbContext _context = context;
        private readonly IStringLocalizer _localizer = localizer;

        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_currentUser.GetUserId());

            if (user == null)
                return Result<bool>.Failure(_localizer["UserNotfound"], 404);

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                return Result<bool>.Failure(_localizer["Incorrectcurrentpassword"], 400);

            var tokens = _context.RefreshTokens.Where(x => x.UserId == user.Id);
            _context.RefreshTokens.RemoveRange(tokens);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, _localizer["Passwordchangedsuccessfully"]);
        }
    }
}