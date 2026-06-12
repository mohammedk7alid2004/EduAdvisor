namespace EduAdvisor.Application.Commands.CourseModules;

public sealed record DeleteCoursesCommand(
 IReadOnlyCollection<Guid> CourseIds)
 : IRequest<Result<bool>>;
