using EduAdvisor.Application.Commands.CourseModules;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseModules;

public sealed class ToggleCourseStatusCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ToggleCourseStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ToggleCourseStatusCommand request,
        CancellationToken cancellationToken)
    {
        var course = await context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            return Result<bool>.NotFound("Course not found.");

        if (course.IsDeleted)
            course.Restore();
        else
            course.SoftDelete();

        await context.SaveChangesAsync(cancellationToken);

        var message = course.IsDeleted
            ? "Course deactivated successfully."
            : "Course activated successfully.";

        return Result<bool>.Success(true, message);
    }
}