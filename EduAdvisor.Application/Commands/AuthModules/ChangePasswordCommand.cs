using MediatR;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record ChangePasswordCommand(string CurrentPassword, string NewPassword, string ConfirmPassword)
        : IRequest<Result<bool>>;
}