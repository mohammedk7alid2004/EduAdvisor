using EduAdvisor.Application.DTO.Auth;
using EduAdvisor.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Identity;

namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class RefreshTokenCommandHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        IStringLocalizer localizer
    ) : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponseDto>>
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IStringLocalizer _localizer = localizer;

        public async Task<Result<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {

            var existingToken = await _tokenService.GetRefreshTokenAsync(request.RefreshToken);

            if (existingToken == null)
                return Result<RefreshTokenResponseDto>.Failure(_localizer["Invalidrefreshtoken"], 400);

            if (existingToken.RevokedOn != null)
                return Result<RefreshTokenResponseDto>.Failure(_localizer["Revokedrefreshtoken"], 400);

            if (existingToken.ExpiresOn <= DateTime.Now)
                return Result<RefreshTokenResponseDto>.Failure(_localizer["Expiredrefreshtoken"], 400);

            var user = await _userManager.FindByIdAsync(existingToken.UserId);

            if (user == null)
                return Result<RefreshTokenResponseDto>.Failure(_localizer["UserNotfound"], 404);

            var jwt = await _tokenService.GenerateJwtToken(user);

            await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);

            var newRefreshToken = await _tokenService.GenerateRefreshToken(user);

            var response = new RefreshTokenResponseDto
            {
                Token = jwt.Token,
                TokenExpiresAt = jwt.ExpiresAt,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresOn
            };

            return Result<RefreshTokenResponseDto>.Success(response, _localizer["Tokenrefreshedsuccessfully"]);
        }
    }
}