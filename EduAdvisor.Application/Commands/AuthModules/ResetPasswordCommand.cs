using MediatR;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword, string ConfirmPassword)
        : IRequest<Result<bool>>;
}