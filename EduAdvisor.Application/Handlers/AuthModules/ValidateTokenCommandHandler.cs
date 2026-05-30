using EduAdvisor.Application.DTO.Auth;

namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class ValidateTokenCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IStringLocalizer localizer
    ) : IRequestHandler<ValidateTokenCommand, Result<ValidateTokenResponseDto>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IStringLocalizer _localizer = localizer;

        public async Task<Result<ValidateTokenResponseDto>> Handle(
            ValidateTokenCommand request,
            CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !(user.Identity?.IsAuthenticated ?? false))
            {
                return Result<ValidateTokenResponseDto>.Failure(_localizer["Expiredtoken"], 401);
            }
            var dto = new ValidateTokenResponseDto(true);
            return Result<ValidateTokenResponseDto>.Success(dto, _localizer["Operationcompletedsuccessfully"], 200);
        }
    }
}