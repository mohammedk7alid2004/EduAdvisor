namespace EduAdvisor.Application.Commands.Departments;

public sealed record DeleteDepartmentCommand
(
    Guid Id
):IRequest<Result<bool>>;