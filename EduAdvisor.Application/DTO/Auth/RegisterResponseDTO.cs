using EduAdvisor.Application.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduAdvisor.Application.DTO.Auth
{
    public record RegisterResponseDTO(
       string Message,
       UserResponseDto User
   );

}
