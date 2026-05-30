using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.DTO.AuthModules;
using EduAdvisor.Application.Interfaces.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public class LoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    IStringLocalizer localizer)
    : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    public async Task<Result<LoginResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        #region Validate User

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result<LoginResponseDto>.Failure(localizer["UserNotFound"], 404);

        if (!user.EmailConfirmed)
            return Result<LoginResponseDto>.Failure(localizer["AccountNotVerified"], 400);

        if (user.IsDisabled)
            return Result<LoginResponseDto>.Failure(localizer["AccountDisabled"], 400);

        if (user.LockoutEnabled && user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            return Result<LoginResponseDto>.Failure(localizer["AccountLocked"], 400);

        var validPassword = await userManager.CheckPasswordAsync(user, request.Password);

        if (!validPassword)
            return Result<LoginResponseDto>.Failure(localizer["InvalidCredentials"], 400);

        #endregion

        #region Generate Tokens

        var jwt = await tokenService.GenerateJwtToken(user);
        var refreshToken = await tokenService.GenerateRefreshToken(user);

        #endregion

        #region Get Role

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        #endregion

        #region Build Response

        var response = new LoginResponseDto
        {
            AccessToken = jwt.Token,
            AccessTokenExpiresAt = jwt.ExpiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresOn,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                IsVerified = user.EmailConfirmed,
                Role = role
            }
        };

        #endregion

        return Result<LoginResponseDto>.Success(response, localizer["LoginSuccessful"]);
    }
}