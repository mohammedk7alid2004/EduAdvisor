namespace EduAdvisor.Application.Queries.Users
{
    public record GetUserRolesQuery(string UserId) : IRequest<Result<List<string>>>;
}
