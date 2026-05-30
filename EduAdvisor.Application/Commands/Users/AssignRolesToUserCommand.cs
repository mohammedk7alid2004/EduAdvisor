namespace EduAdvisor.Application.Commands.Users
{
        public record AssignRolesToUserCommand(
            string UserId,
            List<string> Roles
        ) : IRequest<Result<bool>>;
    }

