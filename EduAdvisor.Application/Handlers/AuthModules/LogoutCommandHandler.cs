using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class LogoutCommandHandler(
        ITokenService tokenService,
        IStringLocalizer localizer
    ) : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly IStringLocalizer _localizer = localizer;

        public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Result<bool>.Failure(_localizer["Refreshtokenrequired"], 400);

            var refreshToken = await _tokenService.GetRefreshTokenAsync(request.RefreshToken);

            if (refreshToken == null)
                return Result<bool>.Failure(_localizer["Invalidrefreshtoken"], 400);

            if (refreshToken.RevokedOn != null)
                return Result<bool>.Failure(_localizer["Refreshtokenalreadyrevoked"], 400);

            await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);

            return Result<bool>.Success(true, _localizer["Loggedoutsuccessfully"]);
        }
    }
}