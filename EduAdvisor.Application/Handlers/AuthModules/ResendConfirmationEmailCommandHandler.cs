using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class ResendConfirmationEmailCommandHandler(
      UserManager<User> userManager,
      IMemoryCache memoryCache,
      IEmailService emailService,
      IHasherService hasher,
      IStringLocalizer localizer
  ) : IRequestHandler<ResendConfirmationEmailCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            ResendConfirmationEmailCommand request,
            CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return Result<bool>.Failure(localizer["UserNotFound"], 404);

            if (user.EmailConfirmed)
                return Result<bool>.Failure(localizer["EmailAlreadyConfirmed"], 400);

            var otp = new Random().Next(100000, 999999).ToString();

            memoryCache.Set(
                $"EmailOTP_{request.Email}",
                hasher.Hash(otp),
                TimeSpan.FromMinutes(5));

            await emailService.SendConfirmationEmail(user, otp);

            return Result<bool>.Success(true, localizer["OTPResent"]);
        }
    }
}