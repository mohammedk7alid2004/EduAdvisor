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
    public class ConfirmEmailCommandHandler(UserManager<User> userManager
        , IStringLocalizer localizer
          , IMemoryCache memoryCache
        ,IHasherService hasherService
        ) : IRequestHandler<ConfirmEmailCommand, Result<bool>>
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IStringLocalizer _localizer = localizer;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IHasherService _hasherService = hasherService;

        public async Task<Result<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Result<bool>.Failure(_localizer["UserNotfound"], 404);
            if (user.EmailConfirmed)
                return Result<bool>.Failure(_localizer["EmailAlreadyConfirmed"], 400);

            var cacheKey = $"EmailOTP_{request.Email}";
            var cachedOtp = _memoryCache.Get<string>(cacheKey);
            if (cachedOtp == null)
                return Result<bool>.Failure(_localizer["OtpExpired"], 400);

            if (!_hasherService.Verify(cachedOtp, request.OTP))
                return Result<bool>.Failure(_localizer["InvalidOTP"], 400);
            _memoryCache.Remove(cacheKey);

            user.EmailConfirmed = true;
                var result = await _userManager.UpdateAsync(user);

            return result.Succeeded ? Result<bool>.Success(true, _localizer["Emailconfirmed"]) 
                : Result<bool>.Failure(_localizer["Emailconfirmationfailed"], 500);
        }
    }
}
