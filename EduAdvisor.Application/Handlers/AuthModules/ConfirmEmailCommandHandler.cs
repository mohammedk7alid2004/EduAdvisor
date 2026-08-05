using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public sealed class ConfirmEmailCommandHandler(
    UserManager<User> userManager,
    IStringLocalizer localizer,
    IOtpService otpService)
    : IRequestHandler<ConfirmEmailCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result<bool>.Failure(
                localizer["UserNotFound"],
                404);

        if (user.EmailConfirmed)
            return Result<bool>.Failure(
                localizer["EmailAlreadyConfirmed"],
                400);

        var isOtpValid = await otpService.ValidateAsync(
            request.Email,
            request.OTP,
            OtpType.EmailConfirmation,
            cancellationToken);

        if (!isOtpValid)
            return Result<bool>.Failure(
                localizer["InvalidOTP"],
                400);

        user.EmailConfirmed = true;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return Result<bool>.Failure(
                localizer["EmailConfirmationFailed"],
                500);

        return Result<bool>.Success(
            true,
            localizer["EmailConfirmed"]);
    }
}