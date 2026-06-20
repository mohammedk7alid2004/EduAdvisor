using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.Student;
using MediatR;

namespace EduAdvisor.Application.Queries.Student;

public sealed record GetAllStudentsQuery(
    string? Search = null,
    Guid? DepartmentId = null,
    Guid? AdvisorId = null,
    int? AcademicYear = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<StudentResponseDto>>>;