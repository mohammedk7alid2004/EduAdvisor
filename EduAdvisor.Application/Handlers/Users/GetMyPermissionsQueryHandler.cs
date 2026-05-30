using EduAdvisor.Application.Queries.Users;

namespace EduAdvisor.Application.Handlers.Users
{
    public class GetMyPermissionsQueryHandler(
       IApplicationDbContext dbContext,
        IGetCurrentUserRepository getCurrentUserRepository
    ) : IRequestHandler<GetMyPermissionsQuery, Result<List<string>>>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IGetCurrentUserRepository _getCurrentUserRepository = getCurrentUserRepository;

        public async Task<Result<List<string>>> Handle(GetMyPermissionsQuery request, CancellationToken cancellationToken)
        {
            var userId = _getCurrentUserRepository.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Result<List<string>>.Failure("Unauthorized", 401);

            var roleIds = await _dbContext.UserRoles
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .ToListAsync();

            var permissions = await _dbContext.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.Permission.PermissionName)
                .Distinct()
                .ToListAsync();

            return Result<List<string>>.Success(permissions, "Operationcompletedsuccessfully", 200);
        }
    }
}
