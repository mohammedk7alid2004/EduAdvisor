using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.DTO.AuthModules;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public sealed class LoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    IApplicationDbContext context,
    IStringLocalizer<LoginCommandHandler> localizer)
    : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    public async Task<Result<LoginResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        #region Validate User

        var email = request.Email.Trim();

        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result<LoginResponseDto>.Failure(
                localizer["UserNotFound"],
                404);
        }

        if (!user.EmailConfirmed)
        {
            return Result<LoginResponseDto>.Failure(
                localizer["AccountNotVerified"],
                400);
        }

        if (user.IsDisabled)
        {
            return Result<LoginResponseDto>.Failure(
                localizer["AccountDisabled"],
                400);
        }

        if (user.LockoutEnabled &&
            user.LockoutEnd is not null &&
            user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            return Result<LoginResponseDto>.Failure(
                localizer["AccountLocked"],
                400);
        }

        var validPassword = await userManager.CheckPasswordAsync(
            user,
            request.Password);

        if (!validPassword)
        {
            return Result<LoginResponseDto>.Failure(
                localizer["InvalidCredentials"],
                400);
        }

        #endregion

        #region Validate Advisor Status

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        if (role == "Advisor")
        {
            var isPending = await context.Advisors
                .AsNoTracking()
                .AnyAsync(
                    advisor => advisor.UserId == user.Id && advisor.IsPending,
                    cancellationToken);

            if (isPending)
            {
                return Result<LoginResponseDto>.Forbidden(
                    localizer["AdvisorPendingApproval"]);
            }
        }

        #endregion

        #region Generate Tokens

        var tokenResult = await tokenService.GenerateTokenAsync(
            user,
            cancellationToken);

        #endregion

        #region Build Response

        var response = new LoginResponseDto
        {
            AccessToken = tokenResult.Token,
            AccessTokenExpiresAt = tokenResult.ExpiresAt,
            RefreshToken = tokenResult.RefreshToken,
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7),
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

        return Result<LoginResponseDto>.Success(
            response,
            localizer["LoginSuccessful"]);
    }
}