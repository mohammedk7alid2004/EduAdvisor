using EduAdvisor.Application.Commands.CourseModules;
using EduAdvisor.Domain.Entities.AcademicModule;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseModules;

public sealed class UpdateCourseCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateCourseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateCourseCommand request,
        CancellationToken cancellationToken)
    {
        var course = await context.Courses
            .Include(c => c.Prerequisites)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            return Result<bool>.NotFound("Course not found.");

        var duplicateExists = await context.Courses
            .AsNoTracking()
            .AnyAsync(c =>
                c.Id != request.CourseId &&
                c.CourseName == request.CourseName,
                cancellationToken);

        if (duplicateExists)
            return Result<bool>.Conflict(
                "A course with the same name already exists.");

        if (request.DepartmentId.HasValue)
        {
            var departmentExists = await context.Departments
                .AsNoTracking()
                .AnyAsync(d => d.Id == request.DepartmentId.Value, cancellationToken);

            if (!departmentExists)
                return Result<bool>.NotFound("Department not found.");
        }

        if (request.PrerequisiteCourseIds.Count != 0)
        {
            var selfReference = request.PrerequisiteCourseIds
                .Any(id => id == request.CourseId);

            if (selfReference)
                return Result<bool>.Conflict(
                    "A course cannot be its own prerequisite.");

            var foundIds = await context.Courses
                .AsNoTracking()
                .Where(c => request.PrerequisiteCourseIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var missingIds = request.PrerequisiteCourseIds
                .Except(foundIds)
                .ToList();

            if (missingIds.Count != 0)
                return Result<bool>.NotFound(
                    $"The following prerequisite courses were not found: {string.Join(", ", missingIds)}");
        }

        course.UpdateDetails(
            request.CourseName,
            request.Description,
            request.CreditHours,
            request.Type,
            request.StandardLevel,
            request.StandardSemester,
            request.DepartmentId);

        var incomingIds = request.PrerequisiteCourseIds.ToHashSet();
        var existingIds = course.Prerequisites
            .Select(p => p.PrerequisiteCourseId)
            .ToHashSet();

        var toRemove = course.Prerequisites
            .Where(p => !incomingIds.Contains(p.PrerequisiteCourseId))
            .ToList();

        var toAdd = incomingIds
            .Except(existingIds)
            .Select(prerequisiteCourseId => new CoursePrerequisite
            {
                CourseId = course.Id,
                PrerequisiteCourseId = prerequisiteCourseId
            })
            .ToList();

        if (toRemove.Count != 0)
            context.CoursePrerequisites.RemoveRange(toRemove);

        if (toAdd.Count != 0)
            context.CoursePrerequisites.AddRange(toAdd);

        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Course updated successfully.");
    }
}