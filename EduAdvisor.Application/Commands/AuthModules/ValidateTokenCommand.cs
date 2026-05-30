using EduAdvisor.Application.DTO.Auth;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record ValidateTokenCommand() : IRequest<Result<ValidateTokenResponseDto>>;
}