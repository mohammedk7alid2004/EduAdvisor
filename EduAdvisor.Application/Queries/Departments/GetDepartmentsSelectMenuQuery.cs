using EduAdvisor.Application.DTO.Common;

namespace EduAdvisor.Application.Queries.Departments;

public sealed record GetDepartmentsSelectMenuQuery(
    Guid? FacultyId
) : IRequest<Result<List<SelectMenuResponse>>>;