using MediatR;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record VerifyResetOtpCommand(string Email, string Otp)
        : IRequest<Result<string>>;
}