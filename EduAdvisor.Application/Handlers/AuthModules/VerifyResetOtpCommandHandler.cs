using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class VerifyResetOtpCommandHandler(
        ITokenService tokenService,
        UserManager<User> userManager,
        IMemoryCache memoryCache,
        IHasherService hasher,
        IStringLocalizer localizer
    ) : IRequestHandler<VerifyResetOtpCommand, Result<string>>
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IHasherService _hasher = hasher;
        private readonly IStringLocalizer _localizer = localizer;

        public async Task<Result<string>> Handle(VerifyResetOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Result<string>.Failure(_localizer["UserNotfound"], 404);

            var cacheKey = $"ResetOTP_{request.Email}";
            var cachedOtp = _memoryCache.Get<string>(cacheKey);

            if (cachedOtp == null)
                return Result<string>.Failure(_localizer["Expiredtoken"], 400);

            if (!_hasher.Verify(cachedOtp, request.Otp))
                return Result<string>.Failure(_localizer["Invalidtoken"], 400);

            _memoryCache.Remove(cacheKey);

            var resetToken = Guid.NewGuid().ToString();
            var hashed = _hasher.Hash(resetToken);
            _memoryCache.Set($"ResetToken_{request.Email}", hashed, TimeSpan.FromMinutes(10));

            return Result<string>.Success(resetToken, _localizer["Otpverifiedsuccessfully"]);
        }
    }
}