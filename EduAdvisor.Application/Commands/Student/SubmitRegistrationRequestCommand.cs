namespace EduAdvisor.Application.Commands.Student;

public sealed record SubmitRegistrationRequestCommand(
   List<Guid> SemesterCourseIds
) : IRequest<Result<Guid>>;
