using EduAdvisor.Application.DTO.Auth;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.AuthModules;

namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class GetRolesQueryHandler(
       IApplicationDbContext context
    ) : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
    {
        public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await context.Roles
                .Select(r => new RoleDto(r.Id, r.Name!))
                .ToListAsync();

            return Result<List<RoleDto>>.Success(roles, "Operationcompletedsuccessfully", 200);
        }
    }
}
