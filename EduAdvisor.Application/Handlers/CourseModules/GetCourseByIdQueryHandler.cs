using EduAdvisor.Application.DTO.CourseModules;
using EduAdvisor.Application.Queries.CourseModules;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseModules;

public sealed class GetCourseByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCourseByIdQuery, Result<CourseDetailsDto>>
{
    public async Task<Result<CourseDetailsDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var course = await context.Courses
            .AsNoTracking()
            .Include(c => c.Department)
            .Include(c => c.CreatedBy)
            .Include(c => c.UpdatedBy)
            .Include(c => c.Prerequisites)
                .ThenInclude(p => p.PrerequisiteCourse)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            return Result<CourseDetailsDto>.NotFound("Course not found.");

        var dto = new CourseDetailsDto(
            course.Id,
            course.CourseCode,
            course.CourseName,
            course.Description,
            course.CreditHours,
            course.Type.ToString(),
            course.StandardLevel,
            course.StandardSemester,
            course.Department?.Name,
            course.IsDeleted,
            course.DeletedAt,
            course.DeletedById,
            course.CreatedBy?.FullName,
            course.CreatedAt,
            course.UpdatedBy?.FullName,
            course.UpdatedAt,
            course.Prerequisites.Select(p => new CoursePrerequisiteDto(
                p.PrerequisiteCourseId,
                p.PrerequisiteCourse.CourseCode,
                p.PrerequisiteCourse.CourseName)));

        return Result<CourseDetailsDto>.Success(dto);
    }
}