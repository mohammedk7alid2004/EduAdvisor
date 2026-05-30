using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.AuthModules;

namespace EduAdvisor.Application.Handlers.AuthModules
{
    public class GetPermissionsQueryHandler(
        IApplicationDbContext context
    ) : IRequestHandler<GetPermissionsQuery, Result<List<string>>>
    {
        public async Task<Result<List<string>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await context.Permissions
                .Select(p => p.PermissionName)
                .ToListAsync();

            return Result<List<string>>.Success(permissions, "Success", 200);
        }
    }
}
