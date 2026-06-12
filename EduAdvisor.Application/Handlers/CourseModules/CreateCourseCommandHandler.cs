using EduAdvisor.Application.Commands.CourseModules;
using EduAdvisor.Domain.Entities.AcademicModule;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseModules;

public sealed class CreateCourseCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateCourseCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken)
    {
        var duplicateExists = await context.Courses
            .AsNoTracking()
            .AnyAsync(
                c => c.CourseCode == request.CourseCode ||
                     c.CourseName == request.CourseName,
                cancellationToken);

        if (duplicateExists)
            return Result<Guid>.Conflict(
                "A course with the same code or name already exists.");

        if (request.DepartmentId.HasValue)
        {
            var departmentExists = await context.Departments
                .AsNoTracking()
                .AnyAsync(d => d.Id == request.DepartmentId.Value, cancellationToken);

            if (!departmentExists)
                return Result<Guid>.NotFound("Department not found.");
        }

        if (request.PrerequisiteCourseIds.Count != 0)
        {
            var foundIds = await context.Courses
                .AsNoTracking()
                .Where(c => request.PrerequisiteCourseIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var missingIds = request.PrerequisiteCourseIds
                .Except(foundIds)
                .ToList();

            if (missingIds.Count != 0)
                return Result<Guid>.NotFound(
                    $"The following prerequisite courses were not found: {string.Join(", ", missingIds)}");
        }

        var course = new Course(
            request.CourseCode,
            request.CourseName,
            request.Description,
            request.CreditHours,
            request.Type,
            request.StandardLevel,
            request.StandardSemester,
            request.DepartmentId);

        context.Courses.Add(course);

        if (request.PrerequisiteCourseIds.Count != 0)
        {
            var prerequisites = request.PrerequisiteCourseIds
                .Select(prerequisiteCourseId => new CoursePrerequisite
                {
                    CourseId = course.Id,
                    PrerequisiteCourseId = prerequisiteCourseId
                });

            context.CoursePrerequisites.AddRange(prerequisites);
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(course.Id, "Course created successfully.");
    }
}