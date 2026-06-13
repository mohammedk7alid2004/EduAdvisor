using EduAdvisor.Application.DTO.CourseAcademicPlans;
using EduAdvisor.Application.Queries.CourseAcademicPlans;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseAcademicPlans;

public sealed class GetCourseAcademicPlanByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCourseAcademicPlanByIdQuery, Result<CourseAcademicPlanDetailsDto>>
{
    public async Task<Result<CourseAcademicPlanDetailsDto>> Handle(
        GetCourseAcademicPlanByIdQuery request,
        CancellationToken cancellationToken)
    {
        var plan = await context.CourseAcademicPlans
            .AsNoTracking()
            .Include(p => p.Course)
            .Include(p => p.Department)
            .Include(p => p.CreatedBy)
            .Include(p => p.UpdatedBy)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (plan is null)
            return Result<CourseAcademicPlanDetailsDto>.NotFound("Course academic plan not found.");

        var dto = new CourseAcademicPlanDetailsDto(
            plan.Id,
            plan.CourseId,
            plan.Course.CourseCode,
            plan.Course.CourseName,
            plan.Level,
            plan.StandardSemester,
            plan.DepartmentId,
            plan.Department?.Name,
            plan.CreatedBy?.UserName,
            plan.CreatedAt,
            plan.UpdatedBy?.UserName,
            plan.UpdatedAt);

        return Result<CourseAcademicPlanDetailsDto>.Success(dto);
    }
}