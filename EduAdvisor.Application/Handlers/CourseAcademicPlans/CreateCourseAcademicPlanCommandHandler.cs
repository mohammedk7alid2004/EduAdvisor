using EduAdvisor.Application.Commands.CourseAcademicPlans;
using EduAdvisor.Domain.Entities.AcademicModule;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseAcademicPlans;

public sealed class CreateCourseAcademicPlanCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateCourseAcademicPlanCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateCourseAcademicPlanCommand request,
        CancellationToken cancellationToken)
    {
        var courseExists = await context.Courses
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.CourseId, cancellationToken);

        if (!courseExists)
            return Result<Guid>.NotFound("Course not found.");

        if (request.DepartmentId.HasValue)
        {
            var departmentExists = await context.Departments
                .AsNoTracking()
                .AnyAsync(d => d.Id == request.DepartmentId.Value, cancellationToken);

            if (!departmentExists)
                return Result<Guid>.NotFound("Department not found.");
        }

        var duplicateExists = await context.CourseAcademicPlans
            .AsNoTracking()
            .AnyAsync(p =>
                p.CourseId == request.CourseId &&
                p.Level == request.Level &&
                p.StandardSemester == request.StandardSemester &&
                p.DepartmentId == request.DepartmentId,
                cancellationToken);

        if (duplicateExists)
            return Result<Guid>.Conflict(
                "A course academic plan with the same course, level, semester, and department already exists.");

        var plan = new CourseAcademicPlan(
            request.CourseId,
            request.Level,
            request.StandardSemester,
            request.DepartmentId);

        context.CourseAcademicPlans.Add(plan);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(plan.Id, "Course academic plan created successfully.");
    }
}