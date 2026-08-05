using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public sealed class ChangePasswordCommandHandler(
    UserManager<User> userManager,
    IGetCurrentUserRepository currentUser,
    IApplicationDbContext context,
    IStringLocalizer localizer)
    : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(currentUser.GetUserId());

        if (user is null)
            return Result<bool>.Failure(
                localizer["UserNotFound"],
                404);

        var result = await userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
            return Result<bool>.Failure(
                localizer["IncorrectCurrentPassword"],
                400);

        var refreshTokens = context.RefreshTokens
            .Where(x => x.UserId == user.Id);

        context.RefreshTokens.RemoveRange(refreshTokens);

        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(
            true,
            localizer["PasswordChangedSuccessfully"]);
    }
}