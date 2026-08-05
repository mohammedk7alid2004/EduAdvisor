using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public sealed class ResetPasswordCommandHandler(
    UserManager<User> userManager,
    IOtpService otpService,
    IApplicationDbContext context,
    IStringLocalizer localizer)
    : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            return Result<bool>.Failure(
                localizer["PasswordMismatch"],
                400);
        }

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result<bool>.Failure(
                localizer["UserNotFound"],
                404);
        }

        var isValidOtp = await otpService.ValidateAsync(
            request.Email,
            request.Otp,
            OtpType.PasswordReset,
            cancellationToken);

        if (!isValidOtp)
        {
            return Result<bool>.Failure(
                localizer["InvalidOtp"],
                400);
        }

        var identityResetToken =
            await userManager.GeneratePasswordResetTokenAsync(user);

        var result = await userManager.ResetPasswordAsync(
            user,
            identityResetToken,
            request.NewPassword);

        if (!result.Succeeded)
        {
            var error = result.Errors.First().Description;

            return Result<bool>.Failure(
                error,
                400);
        }

        var refreshTokens = await context.RefreshTokens
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        context.RefreshTokens.RemoveRange(refreshTokens);

        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(
            true,
            localizer["PasswordResetSuccessfully"]);
    }
}