using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduAdvisor.Application.Commands.AuthModules
{
    public record ConfirmEmailCommand(string Email, string OTP)
         : IRequest<Result<bool>>;
}

