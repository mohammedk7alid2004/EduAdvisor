using EduAdvisor.Application.DTO.Auth;
using MediatR;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record CreateRoleCommand(
       string name
    ) : IRequest<Result<string>>;
}
