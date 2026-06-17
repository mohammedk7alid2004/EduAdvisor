using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public sealed class LogoutCommandHandler(
    ITokenService tokenService,
    IStringLocalizer<LogoutCommandHandler> localizer)
    : IRequestHandler<LogoutCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var refreshToken = request.RefreshToken?.Trim();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result<bool>.Failure(
                localizer["RefreshTokenRequired"],
                400);
        }

        var revokeResult = await tokenService.RevokeRefreshTokenAsync(
            refreshToken,
            cancellationToken);

        return revokeResult switch
        {
            RevokeRefreshTokenResult.Success =>
                Result<bool>.Success(
                    true,
                    localizer["LoggedOutSuccessfully"]),

            RevokeRefreshTokenResult.NotFound =>
                Result<bool>.Failure(
                    localizer["InvalidRefreshToken"],
                    400),

            RevokeRefreshTokenResult.AlreadyRevoked =>
                Result<bool>.Failure(
                    localizer["RefreshTokenAlreadyRevoked"],
                    400),

            RevokeRefreshTokenResult.Expired =>
                Result<bool>.Failure(
                    localizer["RefreshTokenExpired"],
                    400),

            _ =>
                Result<bool>.Failure(
                    localizer["UnexpectedError"],
                    500)
        };
    }
}