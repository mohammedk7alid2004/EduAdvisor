using EduAdvisor.Application.DTO.Departments;

namespace EduAdvisor.Application.Commands.Departments;

public sealed record CreateDepartmentCommand
(
    Guid FacultyId,
    string Name,
    string? Code,
    string? Description
):IRequest<Result<DepartmentsResponse>>;
