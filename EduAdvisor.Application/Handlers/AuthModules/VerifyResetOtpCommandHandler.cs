using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public sealed class VerifyResetOtpCommandHandler(
    UserManager<User> userManager,
    IOtpService otpService,
    IMemoryCache memoryCache,
    IHasherService hasherService,
    IStringLocalizer localizer)
    : IRequestHandler<VerifyResetOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        VerifyResetOtpCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result<string>.Failure(
                localizer["UserNotFound"],
                404);

        var isValid = await otpService.ValidateAsync(
            request.Email,
            request.Otp,
            OtpType.PasswordReset,
            cancellationToken);

        if (!isValid)
            return Result<string>.Failure(
                localizer["InvalidToken"],
                400);

        var resetToken = Guid.NewGuid().ToString("N");

        memoryCache.Set(
            $"ResetToken_{request.Email.ToLowerInvariant()}",
            hasherService.Hash(resetToken),
            TimeSpan.FromMinutes(10));

        return Result<string>.Success(
            resetToken,
            localizer["OtpVerifiedSuccessfully"]);
    }
}