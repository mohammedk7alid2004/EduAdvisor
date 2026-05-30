using EduAdvisor.Application.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduAdvisor.Application.Queries.Users
{
    public record GetCurrentUserProfileQuery() : IRequest<Result<CurrentUserResponseDTO>>;
}
