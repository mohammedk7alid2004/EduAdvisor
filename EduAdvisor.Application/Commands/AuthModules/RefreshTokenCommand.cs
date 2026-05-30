using EduAdvisor.Application.DTO.Auth;
using MediatR;

namespace EduAdvisor.Application.Commands.AuthModules
{
 
        public record RefreshTokenCommand(string RefreshToken)
            : IRequest<Result<RefreshTokenResponseDto>>;
    }

