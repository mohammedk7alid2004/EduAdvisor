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

public sealed class ForgotPasswordCommandHandler(
    UserManager<User> userManager,
    IOtpService otpService,
    IStringLocalizer localizer,
    IBackgroundJobClient backgroundJobClient)
    : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result<bool>.Failure(
                localizer["UserNotFound"],
                404);

        var otp = await otpService.GenerateAndStoreAsync(
            request.Email,
            OtpType.PasswordReset,
            cancellationToken);

        backgroundJobClient.Enqueue<IEmailService>(
            service => service.SendResetPasswordEmail(user, otp));

        return Result<bool>.Success(
            true,
            localizer["PasswordResetInstructionsSent"]);
    }
}