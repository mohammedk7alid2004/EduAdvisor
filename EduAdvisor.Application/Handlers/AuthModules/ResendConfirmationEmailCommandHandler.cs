using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public sealed class ResendConfirmationEmailCommandHandler(
    UserManager<User> userManager,
    IOtpService otpService,
    IStringLocalizer localizer,
    IBackgroundJobClient backgroundJobClient)
    : IRequestHandler<ResendConfirmationEmailCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ResendConfirmationEmailCommand request,
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

        var otp = await otpService.GenerateAndStoreAsync(
            request.Email,
            OtpType.EmailConfirmation,
            cancellationToken);

        backgroundJobClient.Enqueue<IEmailService>(
            service => service.SendConfirmationEmail(user, otp));

        return Result<bool>.Success(
            true,
            localizer["OTPResent"]);
    }
}