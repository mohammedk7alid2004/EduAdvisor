namespace EduAdvisor.Application.Commands.AuthModules;

public record CreateRoleCommand(
   string Name,
  List<Guid> PermissionIds
) : IRequest<Result<string>>;
