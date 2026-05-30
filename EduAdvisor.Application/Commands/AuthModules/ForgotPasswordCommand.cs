using MediatR;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record ForgotPasswordCommand(string Email)
        : IRequest<Result<bool>>;
}