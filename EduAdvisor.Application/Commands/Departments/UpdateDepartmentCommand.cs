namespace EduAdvisor.Application.Commands.Departments;

public sealed record UpdateDepartmentCommand
(
    Guid Id,
    Guid? FacultyId,
    string? Name,
    string? Code,
    string? Description
):IRequest<Result<bool>>;