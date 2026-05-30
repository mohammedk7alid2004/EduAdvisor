using EduAdvisor.Application.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record UpdateRoleCommand(
     string Id,
     string Name
    ) : IRequest<Result<string>>;
}
