using MediatR;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record LogoutCommand(string RefreshToken)
        : IRequest<Result<bool>>;
}