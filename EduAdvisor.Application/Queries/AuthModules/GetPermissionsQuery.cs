using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduAdvisor.Application.Queries.AuthModules
{
    public record GetPermissionsQuery() : IRequest<Result<List<string>>>;
}
