using EduAdvisor.Application.DTO.Auth;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public sealed class RefreshTokenCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    IStringLocalizer<RefreshTokenCommandHandler> localizer)
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponseDto>>
{
    public async Task<Result<RefreshTokenResponseDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var refreshToken = request.RefreshToken?.Trim();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result<RefreshTokenResponseDto>.Failure(
                localizer["RefreshTokenRequired"],
                400);
        }

        var existingToken = await tokenService.GetRefreshTokenAsync(
            refreshToken,
            cancellationToken);

        if (existingToken is null)
        {
            return Result<RefreshTokenResponseDto>.Failure(
                localizer["InvalidRefreshToken"],
                400);
        }

        if (existingToken.RevokedOn is not null)
        {
            return Result<RefreshTokenResponseDto>.Failure(
                localizer["RevokedRefreshToken"],
                400);
        }

        if (existingToken.ExpiresOn <= DateTime.UtcNow)
        {
            return Result<RefreshTokenResponseDto>.Failure(
                localizer["ExpiredRefreshToken"],
                400);
        }

        var user = await userManager.FindByIdAsync(existingToken.UserId);

        if (user is null)
        {
            return Result<RefreshTokenResponseDto>.Failure(
                localizer["UserNotFound"],
                404);
        }

        var jwt = await tokenService.GenerateJwtTokenAsync(
            user,
            cancellationToken);

        var revokeResult = await tokenService.RevokeRefreshTokenAsync(
            refreshToken,
            cancellationToken);

        if (revokeResult != RevokeRefreshTokenResult.Success)
        {
            return Result<RefreshTokenResponseDto>.Failure(
                localizer["InvalidRefreshToken"],
                400);
        }

        var newRefreshToken = await tokenService.GenerateRefreshTokenAsync(
            user,
            cancellationToken);

        var response = new RefreshTokenResponseDto
        {
            Token = jwt,
            TokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresAt = newRefreshToken.ExpiresOn
        };

        return Result<RefreshTokenResponseDto>.Success(
            response,
            localizer["TokenRefreshedSuccessfully"]);
    }
}