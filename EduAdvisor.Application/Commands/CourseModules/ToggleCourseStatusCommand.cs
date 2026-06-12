namespace EduAdvisor.Application.Commands.CourseModules;

public sealed record ToggleCourseStatusCommand(Guid CourseId)
    : IRequest<Result<bool>>;