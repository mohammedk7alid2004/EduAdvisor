
using EduAdvisor.Application.DTO.User;
using MediatR;

namespace EduAdvisor.Application.Queries.Users;

public sealed class GetAdvisorStudentsQuery
    : IRequest<Result<PaginatedList<AdvisorStudentResponse>>>
{
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}