using EduAdvisor.Application.DTO.CourseModules;
using EduAdvisor.Application.Queries.CourseModules;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.CourseModules;

public sealed class GetCoursesSelectQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCoursesSelectQuery, Result<List<CourseSelectDto>>>
{
    public async Task<Result<List<CourseSelectDto>>> Handle(
        GetCoursesSelectQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Courses
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .AsQueryable();

        if (request.DepartmentId.HasValue)
            query = query.Where(c => c.DepartmentId == request.DepartmentId.Value);

        if (request.StandardLevel.HasValue)
            query = query.Where(c => c.StandardLevel == request.StandardLevel.Value);

        if (request.StandardSemester.HasValue)
            query = query.Where(c => c.StandardSemester == request.StandardSemester.Value);

        if (!string.IsNullOrWhiteSpace(request.CourseType))
            query = query.Where(c => c.Type.ToString() == request.CourseType);

        var result = await query
            .OrderBy(c => c.StandardLevel)
            .ThenBy(c => c.StandardSemester)
            .ThenBy(c => c.CourseName)
            .Select(c => new CourseSelectDto(
                c.Id,
                c.CourseCode,
                c.CourseName,
                c.CreditHours,
                c.Type.ToString(),
                c.StandardLevel,
                c.StandardSemester))
            .ToListAsync(cancellationToken);

        return Result<List<CourseSelectDto>>.Success(result);
    }
}