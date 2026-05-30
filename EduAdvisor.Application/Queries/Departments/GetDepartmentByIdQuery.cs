using EduAdvisor.Application.DTO.Departments;

namespace EduAdvisor.Application.Queries.Departments;

public sealed record GetDepartmentByIdQuery(
    Guid Id
) : IRequest<Result<DescriptionListResponse>>;