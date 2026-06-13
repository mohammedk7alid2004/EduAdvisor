using EduAdvisor.Application.DTO.SemesterCourses;
using EduAdvisor.Application.Queries.SemesterCourses;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.SemesterCourses;

public sealed class GetSemesterCoursesSelectQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSemesterCoursesSelectQuery, Result<List<SemesterCourseSelectDto>>>
{
    public async Task<Result<List<SemesterCourseSelectDto>>> Handle(
        GetSemesterCoursesSelectQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.SemesterCourses
            .AsNoTracking()
            .Include(sc => sc.CourseAcademicPlan)
                .ThenInclude(p => p.Course)
            .Include(sc => sc.CourseAcademicPlan)
                .ThenInclude(p => p.Department)
            .Where(sc => sc.SemesterId == request.SemesterId)
            .AsQueryable();

        if (request.Level.HasValue)
            query = query.Where(sc => sc.CourseAcademicPlan.Level == request.Level.Value);

        if (request.DepartmentId.HasValue)
            query = query.Where(sc => sc.CourseAcademicPlan.DepartmentId == request.DepartmentId.Value);

        var result = await query
            .OrderBy(sc => sc.CourseAcademicPlan.Level)
            .ThenBy(sc => sc.CourseAcademicPlan.Course.CourseName)
            .Select(sc => new SemesterCourseSelectDto(
                sc.Id,
                sc.CourseAcademicPlan.Course.CourseCode,
                sc.CourseAcademicPlan.Course.CourseName,
                sc.CourseAcademicPlan.Course.CreditHours,
                sc.CourseAcademicPlan.Level,
                sc.CourseAcademicPlan.StandardSemester,
                sc.CourseAcademicPlan.Department != null
                    ? sc.CourseAcademicPlan.Department.Name
                    : null))
            .ToListAsync(cancellationToken);

        return Result<List<SemesterCourseSelectDto>>.Success(result);
    }
}