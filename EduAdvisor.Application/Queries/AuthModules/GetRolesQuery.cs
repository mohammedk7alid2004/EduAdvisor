using EduAdvisor.Application.DTO.Auth;


namespace EduAdvisor.Application.Queries.AuthModules
{
    public record GetRolesQuery() : IRequest<Result<List<RoleDto>>>;
}
