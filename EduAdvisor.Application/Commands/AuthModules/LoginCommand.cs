using EduAdvisor.Application.DTO.Auth;
using EduAdvisor.Application.DTO.AuthModules;
using MediatR;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record LoginCommand(string Email, string Password)
        : IRequest<Result<LoginResponseDto>>;
}