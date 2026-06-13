using EduAdvisor.Application.Commands.CourseAcademicPlans;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseAcademicPlans;

public sealed class UpdateCourseAcademicPlanCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateCourseAcademicPlanCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateCourseAcademicPlanCommand request,
        CancellationToken cancellationToken)
    {
        var plan = await context.CourseAcademicPlans
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (plan is null)
            return Result<bool>.NotFound("Course academic plan not found.");

        var courseExists = await context.Courses
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.CourseId, cancellationToken);

        if (!courseExists)
            return Result<bool>.NotFound("Course not found.");

        if (request.DepartmentId.HasValue)
        {
            var departmentExists = await context.Departments
                .AsNoTracking()
                .AnyAsync(d => d.Id == request.DepartmentId.Value, cancellationToken);

            if (!departmentExists)
                return Result<bool>.NotFound("Department not found.");
        }

        var duplicateExists = await context.CourseAcademicPlans
            .AsNoTracking()
            .AnyAsync(p =>
                p.Id != request.Id &&
                p.CourseId == request.CourseId &&
                p.Level == request.Level &&
                p.StandardSemester == request.StandardSemester &&
                p.DepartmentId == request.DepartmentId,
                cancellationToken);

        if (duplicateExists)
            return Result<bool>.Conflict(
                "A course academic plan with the same course, level, semester, and department already exists.");

        plan.Update(request.CourseId, request.Level, request.StandardSemester, request.DepartmentId);

        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Course academic plan updated successfully.");
    }
}