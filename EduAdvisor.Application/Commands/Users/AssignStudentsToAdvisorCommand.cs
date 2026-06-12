using MediatR;

namespace EduAdvisor.Application.Commands.Users;

public sealed class AssignStudentsToAdvisorCommand : IRequest<Result<bool>>
{
    public Guid AdvisorId { get; set; }
    public List<Guid> StudentIds { get; set; } = [];
}