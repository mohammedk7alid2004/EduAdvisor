using MediatR;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record ResendConfirmationEmailCommand(string Email)
      : IRequest<Result<bool>>;
}
