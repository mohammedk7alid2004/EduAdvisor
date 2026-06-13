using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.CourseAcademicPlans;
using EduAdvisor.Application.Queries.CourseAcademicPlans;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseAcademicPlans;

public sealed class GetAllCourseAcademicPlansQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllCourseAcademicPlansQuery, Result<PaginatedList<CourseAcademicPlanListDto>>>
{
    public async Task<Result<PaginatedList<CourseAcademicPlanListDto>>> Handle(
        GetAllCourseAcademicPlansQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.CourseAcademicPlans
            .AsNoTracking()
            .Include(p => p.Course)
            .Include(p => p.Department)
            .Include(p => p.CreatedBy)
            .AsQueryable();

        if (request.Level.HasValue)
            query = query.Where(p => p.Level == request.Level.Value);

        if (request.StandardSemester.HasValue)
            query = query.Where(p => p.StandardSemester == request.StandardSemester.Value);

        if (request.DepartmentId.HasValue)
            query = query.Where(p => p.DepartmentId == request.DepartmentId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(p =>
                p.Course.CourseName.ToLower().Contains(search) ||
                p.Course.CourseCode.ToLower().Contains(search));
        }

        var projected = query.Select(p => new CourseAcademicPlanListDto(
            p.Id,
            p.CourseId,
            p.Course.CourseCode,
            p.Course.CourseName,
            p.Level,
            p.StandardSemester,
            p.Department != null ? p.Department.Name : null,
            p.CreatedBy != null ? p.CreatedBy.UserName : null,
            p.CreatedAt));

        var result = await PaginatedList<CourseAcademicPlanListDto>.CreateAsync(
            projected,
            request.PageNumber,
            request.PageSize
            );

        return Result<PaginatedList<CourseAcademicPlanListDto>>.Success(result);
    }
}