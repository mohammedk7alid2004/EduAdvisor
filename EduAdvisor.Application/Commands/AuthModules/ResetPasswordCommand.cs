using MediatR;

namespace EduAdvisor.Application.Commands.AuthModules;

public sealed record ResetPasswordCommand(
    string Email,
    string Otp,
    string NewPassword,
    string ConfirmPassword)
    : IRequest<Result<bool>>;