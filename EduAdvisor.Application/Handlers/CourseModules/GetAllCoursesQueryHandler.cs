using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.CourseListDto;
using EduAdvisor.Application.Queries.CourseModules;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseModules;

public sealed class GetAllCoursesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllCoursesQuery, Result<PaginatedList<CourseListDto>>>
{
    public async Task<Result<PaginatedList<CourseListDto>>> Handle(
        GetAllCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Courses
            .AsNoTracking()
            .Include(c => c.Department)
            .Include(c => c.CreatedBy)
            .Include(c => c.UpdatedBy)
            .AsQueryable();

        if (request.IsDeleted.HasValue)
            query = query.Where(c => c.IsDeleted == request.IsDeleted.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(c =>
                c.CourseCode.ToLower().Contains(search) ||
                c.CourseName.ToLower().Contains(search));
        }

        var projected = query.Select(c => new CourseListDto(
            c.Id,
            c.CourseCode,
            c.CourseName,
            c.Description,
            c.CreditHours,
            c.Type.ToString(),
            c.StandardLevel,
            c.StandardSemester,
            c.Department != null ? c.Department.Name : null,
            c.IsDeleted,
            c.CreatedBy != null ? c.CreatedBy.FullName : null,
            c.CreatedAt,
            c.UpdatedBy != null ? c.UpdatedBy.FullName : null,
            c.UpdatedAt));

        var result = await PaginatedList<CourseListDto>.CreateAsync(
            projected,
            request.PageNumber,
            request.PageSize
            );

        return Result<PaginatedList<CourseListDto>>.Success(result);
    }
}