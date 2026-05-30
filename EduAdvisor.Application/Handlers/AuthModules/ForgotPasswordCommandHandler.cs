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
    public class ForgotPasswordCommandHandler(
        UserManager<User> userManager,
        IMemoryCache memoryCache,
        IEmailService emailService,
        IHasherService hasher,
        IStringLocalizer localizer
    ) : IRequestHandler<ForgotPasswordCommand, Result<bool>>
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IEmailService _emailService = emailService;
        private readonly IHasherService _hasher = hasher;
        private readonly IStringLocalizer _localizer = localizer;

        public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Result<bool>.Failure(_localizer["UserNotfound"], 404);

            var otp = new Random().Next(100000, 999999).ToString();
            var hashedOtp = _hasher.Hash(otp);

            var cacheKey = $"ResetOTP_{request.Email}";
            _memoryCache.Set(cacheKey, hashedOtp, TimeSpan.FromMinutes(15));

            await _emailService.SendResetPasswordEmail(user, otp);

            return Result<bool>.Success(true, _localizer["Passwordresetinstructionssent"]);
        }
    }
}